using GE.Contracts.DomainModels.Adpters.Interfaces;
using GE.Integration.Shopee.Application.Adapters;
using GE.Integration.Shopee.Application.Aws;
using GE.Integration.Shopee.Domain.Enum;
using GE.Integration.Shopee.Domain.Request;
using GE.Integration.Shopee.Domain.Response.WebHook;
using GE.Integration.Shopee.Infra.External.Products;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GE.Integration.Shopee.Application.Services.Stock
{
    public class StockService : IStockService
    {
        private readonly IGetProductsExternal _productsExternalService;
        private readonly IAwsService _awsService;
        private readonly IStockServiceAdapter _stockServiceAdapter;
        private const string SHOPEE = "SHOPEE";

        private readonly string? _awsStockDetails = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_STOCK_DETAILS"))
            ? throw new Exception("AWS_SHOPEE_STOCK_DETAILS - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("AWS_STOCK_DETAILS");

        private readonly string? _awsStockIntegration = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_STOCK_INTEGRATION_CORE"))
            ? throw new Exception("AWS_SHOPEE_STOCK_INTEGRATION_CORE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("AWS_STOCK_INTEGRATION_CORE");


        public StockService(IGetProductsExternal productsExternalService,
            IAwsService awsService,
            IStockServiceAdapter stockServiceAdapter)
        {
            _productsExternalService = productsExternalService;
            _awsService = awsService;
            _stockServiceAdapter = stockServiceAdapter;
        }

        public async Task<IActionResult> GetItemList(string accessToken, DateTime dateCreatedFrom, DateTime dateCreatedTo, int shopId, CancellationToken cancellationToken)
        {
            try
            {
                var listIdString = string.Empty;
                var nextPage = true;
                var pageSize = 10;
                var page = 0;

                while (nextPage)
                {
                    var listItem = await _productsExternalService.GetItemList(accessToken, shopId, dateCreatedFrom, dateCreatedTo, cancellationToken, page, pageSize);

                    if (listItem.response != null! &&
                        listItem.response.item == null! ||
                        listItem.response.item.Count == 0)
                    {
                        return new BadRequestObjectResult("[Integration.Shopee][External][GetStockList] A lista de stock está vazia.");
                    }

                    listIdString = string.Join(", ", listItem.response.item.Select(item => item.item_id));

                    var message = new WebHookPost
                    {
                        accessToken = accessToken,
                        shopId = shopId,
                        itemIds = listIdString,
                        hashIntegration = SHOPEE,
                        EntityType = ETypeEntity.Stock
                    };

                    var webHookContentMessage = JsonConvert.SerializeObject(message);
                    await _awsService.SendAwsMessage(webHookContentMessage, _awsStockDetails);

                    nextPage = listItem.response.has_next_page;
                    page++;
                }

                return new OkObjectResult(listIdString);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        public async Task<string> GetItemDetail(WebHookPost webHookContentMessage, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(webHookContentMessage.itemIds))
            {
                var itemIds = webHookContentMessage.itemIds.Split(',');

                try
                {
                    foreach (var idProduct in itemIds)
                    {
                        var productVariationList = await _productsExternalService.GetModelItemList(webHookContentMessage.accessToken, webHookContentMessage.shopId, Convert.ToInt64(idProduct), new CancellationToken());

                        if (productVariationList.Response is { model: not null })
                        {
                            foreach (var model in productVariationList.Response.model)
                            {
                                var stockAdapter = new ShopeeToStockAdapter(model, idProduct);

                                var stockDomain = await _stockServiceAdapter.ConvertToStockAdapter(stockAdapter);

                                var objectRequest = new ObjectRequest
                                {
                                    EntityType = ETypeEntity.Stock,
                                    JsonObject = JsonConvert.SerializeObject(stockDomain),
                                    HashIntegracao = SHOPEE
                                };

                                var json = JsonConvert.SerializeObject(objectRequest);

                                await _awsService.SendAwsMessage(json, _awsStockIntegration);

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new Exception(e.Message);
                }
            }

            return "OK";
        }
    }
}
