using GE.Integration.Shopee.Application.Adapters;
using GE.Integration.Shopee.Infra.External.Payment;
using GE.Integration.Shopee.Infra.External.Orders;
using GE.Integration.Shopee.Infra.External.Shop;
using GE.Contracts.DomainModels.Adpters.Interfaces;
using Newtonsoft.Json;
using GE.Integration.Shopee.Application.Aws;
using GE.Integration.Shopee.Domain.Response.Orders;
using GE.Integration.Shopee.Domain.Response.Shop;
using GE.Integration.Shopee.Application.Services.Cache;
using GE.Integration.Shopee.Domain.Enum;
using GE.Integration.Shopee.Domain.Response.WebHook;
using Microsoft.AspNetCore.Mvc;
using GE.Integration.Shopee.Domain.Request;
using GE.Integration.Shopee.Infra.Repositories;
using GE.Integration.Shopee.Model;
using Microsoft.EntityFrameworkCore;

namespace GE.Integration.Shopee.Application.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IGetShopInfoExternal _getShopInfo;
        private readonly IAwsService _awsService;
        private readonly ICacheService _cacheService;
        private readonly IGetOrdersExternal _getOrdersExternal;
        private readonly IOrderServiceAdapter _orderServiceAdapter;
        private readonly IGetEscrowDetailsExternal _getEscrowDetails;
        private const string SHOPEE = "SHOPEE";
        private readonly IARepository<OrderImport> _repositoryOrderImport;


        private readonly string? _awsOrderDetails = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_ORDER_DETAILS"))
            ? throw new Exception("AWS_SHOPEE_ORDER_DETAILS - EMPTY EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("AWS_ORDER_DETAILS");

        private readonly string? _awsOrderIntegration = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_ORDER_INTEGRATION_CORE"))
            ? throw new Exception("AWS_SHOPEE_ORDER_INTEGRATION_CORE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("AWS_ORDER_INTEGRATION_CORE");

        public OrderService(IGetOrdersExternal getOrdersExternal,
            IOrderServiceAdapter orderServiceAdapter,
            IGetEscrowDetailsExternal getEscrowDetails,
            IGetShopInfoExternal getShopInfo,
            IAwsService awsService,
            ICacheService cacheService,
            IARepository<OrderImport> repositoryOrderImport)
        {
            _getShopInfo = getShopInfo;
            _awsService = awsService;
            _cacheService = cacheService;
            _getOrdersExternal = getOrdersExternal;
            _orderServiceAdapter = orderServiceAdapter;
            _getEscrowDetails = getEscrowDetails;
            _repositoryOrderImport = repositoryOrderImport;
        }

        public async Task<IActionResult> GetOrderList(Guid customerId, long? customerPlanId, string accessToken, int shopId, 
            DateTime dateCreatedFrom, DateTime dateCreatedTo, CancellationToken cancellationToken)
        {
            //TODO TALVEZ CRIAR UM CACHE DO TOKEN AQUI PARA USAR NO WEBHOOK DA SHOPEE

            try
            {
                var listIdString = string.Empty;
                var status = new Dictionary<string, string>();
                var more = true;
                var pageSize = 50;
                var cursor = "";

                while (more)
                {
                    var listOrders = await _getOrdersExternal.GetOrderList(accessToken, shopId, dateCreatedFrom, dateCreatedTo, cancellationToken, cursor, pageSize);
                    
                    if (listOrders == null ||
                        listOrders.response == null ||
                        listOrders.response.order_list == null ||
                        listOrders.response.order_list.Count == 0)
                    {
                        return new BadRequestObjectResult("[Integration.Shopee][External][GetOrderList] A lista de pedidos está vazia.");
                    }

                    listIdString = string.Join(", ", listOrders.response.order_list.Select(item => item.order_sn));
                    status = listOrders.response.order_list.ToDictionary(k => k.order_sn, v => v.order_status);

                    var message = new WebHookPost
                    {
                        accessToken = accessToken,
                        shopId = shopId,
                        customerId = customerId,
                        customerPlanId = customerPlanId,
                        itemIds = listIdString,
                        hashIntegration = SHOPEE,
                        EntityType = ETypeEntity.Order,
                        status = status
                    };

                    string jsonString = JsonConvert.SerializeObject(message, Formatting.Indented);

                    // verifica se o status mudou
                    await _awsService.SendAwsMessage(jsonString, _awsOrderDetails);

                    more = listOrders.response.more;
                    cursor = listOrders.response.next_cursor;
                }

                return new OkObjectResult(listIdString);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        public async Task<string> GetOrderDetail(WebHookPost webHookContentMessage, CancellationToken cancellationToken)
        {
            try
            {
                if (webHookContentMessage != null! && !string.IsNullOrEmpty(webHookContentMessage.itemIds))
                {
                    string[] orderIdList = webHookContentMessage.itemIds.Split(',').Select(t => t.Trim()).ToArray();

                    var existingOrders = await _repositoryOrderImport.Queryable()
                    .Where(o => orderIdList.Contains(o.EcommerceNumber)).Select(o => o).ToListAsync(cancellationToken: cancellationToken);

                    foreach (var orderId in orderIdList)
                    {
                        var existingOrder = existingOrders.FirstOrDefault(o => o.EcommerceNumber == orderId);
                        webHookContentMessage.status.TryGetValue(orderId, out var currentStatus);

                        if (existingOrder == null || (currentStatus != null && currentStatus != existingOrder.Status))
                        {
                            var detailsItem = await _getOrdersExternal.GetOrderDetailList(webHookContentMessage.accessToken, webHookContentMessage.shopId, orderId.Trim(), cancellationToken);

                            if (detailsItem.response != null! &&
                                detailsItem.response.order_list != null!)
                            {
                                var escrowDetail = await _getEscrowDetails.GetEscrowDetails(webHookContentMessage.accessToken, webHookContentMessage.shopId, detailsItem.response.order_list[0].order_sn);

                                var shopInfo = await _getShopInfo.GetShopInfoDetails(webHookContentMessage.accessToken, webHookContentMessage.shopId);

                                var orderAdapter = new ShopeeToOrderAdapter(detailsItem.response.order_list[0], escrowDetail, shopInfo);

                                var orderDomain = await _orderServiceAdapter.ConverToOrderAdapter(orderAdapter);
                                orderDomain.CustomerId = webHookContentMessage.customerId;
                                orderDomain.CustomerPlanId = webHookContentMessage.customerPlanId;

                                var objectRequest = new ObjectRequest
                                {
                                    EntityType = ETypeEntity.Order,
                                    JsonObject = JsonConvert.SerializeObject(orderDomain),
                                    HashIntegracao = SHOPEE
                                };

                                var json = JsonConvert.SerializeObject(objectRequest);

                                // Antes de chamar a fila salva no banco local
                                await SaveLocalOrderAsync(existingOrder, orderDomain);

                                await _awsService.SendAwsMessage(json, _awsOrderIntegration);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }

            return "OK";
        }

        private async Task SaveLocalOrderAsync(OrderImport orderDb, Contracts.DomainModels.Orders.Order orderDomain)
        {
            var isNew = orderDb == null;

            orderDb ??= new OrderImport();

            orderDb.CustomerId = orderDomain.CustomerId;
            orderDb.CustomerPlanId = orderDomain.CustomerPlanId;
            orderDb.OrderId = orderDomain.OrderId;
            orderDb.Status = orderDomain.OrderStatus;
            orderDb.EcommerceNumber = string.IsNullOrEmpty(orderDomain.EcommerceNumber) ? orderDomain.OrderId : orderDomain.EcommerceNumber;
            orderDb.SalesChannel = orderDomain.SalesChannel;
            orderDb.OrderDate = orderDomain.OrderDate; // ou outra data relevante

            if (isNew)
            {
                orderDb.CreatedAt = DateTime.Now;
                await _repositoryOrderImport.DbContext.AddAsync(orderDb);
            }
            else
            {
                orderDb.UpdatedAt = DateTime.Now;
            }

            await _repositoryOrderImport.DbContext.SaveChangesAsync();
        }

        public async Task PostWebhookShopee(OrderWebHookShopeePost request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.shop_id != null)
                {

                    //Criar esse cache ao buscar as ordens (utilzar o que vem da chamada externa)
                    var objIntegrationMessage = await _cacheService.GetCacheToken(request.shop_id);

                    if (objIntegrationMessage != null && request.data != null && request.data.ordersn != null)
                    {
                        var detailsItem = await _getOrdersExternal.GetOrderDetailList(objIntegrationMessage.accessToken, request.shop_id, request.data.ordersn, cancellationToken);

                        if (detailsItem != null && detailsItem.response.order_list.Count > 0)
                        {
                            var escrowDetail = await _getEscrowDetails.GetEscrowDetails(objIntegrationMessage.accessToken, objIntegrationMessage.shopId, request.data.ordersn);

                            var shopInfo = await _getShopInfo.GetShopInfoDetails(objIntegrationMessage.accessToken, objIntegrationMessage.shopId);

                            var orderAdapter = new ShopeeToOrderAdapter(detailsItem.response.order_list[0], escrowDetail, shopInfo);
                            var orderDomain = await _orderServiceAdapter.ConverToOrderAdapter(orderAdapter);
                            orderDomain.CustomerId = objIntegrationMessage.customerId;

                            var objectRequest = new ObjectRequest
                            {
                                EntityType = ETypeEntity.Order,
                                JsonObject = JsonConvert.SerializeObject(orderDomain),
                                HashIntegracao = SHOPEE
                            };

                            var json = JsonConvert.SerializeObject(objectRequest);

                            await _awsService.SendAwsMessage(json, _awsOrderIntegration);
                        }
                    }

                    throw new Exception("Erro ao recuperar dados .Request data ou objeto do cache não encontrado.");

                }
                throw new Exception("Erro ao recuperar dados do webhook shopee e envio para a fila SQS. Request data ou shop id não encontrado.");

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao recuperar dados do webhook shopee e envio para a fila SQS. - {ex.Message}");
            }

        }

        public async Task<ShopInfoResponse> GetShopDetail(string accessToken, int shopId)
        {
            try
            {
                return await _getShopInfo.GetShopInfoDetails(accessToken, shopId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> DeleteLocalBiggerThanXDays(int days)
        {
            try
            {
                var deadLine = DateTime.Now.AddDays(-1 * days);

                var items = _repositoryOrderImport.Queryable().Where(t => t.CreatedAt <= deadLine);
                _repositoryOrderImport.DbContext.RemoveRange(items);
                await _repositoryOrderImport.DbContext.SaveChangesAsync();

                return true; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }
    }
}
