using GE.Contracts.DomainModels.Adpters.Interfaces;
using GE.Integration.Shopee.Application.Adapters;
using GE.Integration.Shopee.Application.Aws;
using GE.Integration.Shopee.Domain.Enum;
using GE.Integration.Shopee.Domain.Request;
using GE.Integration.Shopee.Domain.Response.WebHook;
using GE.Integration.Shopee.Infra.External.Products;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GE.Integration.Shopee.Application.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductServiceAdapter _productServiceAdapter;
        private readonly IAwsService _awsService;
        private readonly IGetProductsExternal _productExternalService;
        private const string SHOPEE = "SHOPEE";

        private readonly string? _awsProductDetails = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_PRODUCT_DETAILS"))
            ? throw new Exception("AWS_SHOPEE_PRODUCT_DETAILS - EMPTY EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("AWS_PRODUCT_DETAILS");

        private readonly string? _awsProductIntegration = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_PRODUCT_INTEGRATION_CORE"))
            ? throw new Exception("AWS_SHOPEE_PRODUCT_INTEGRATION_CORE - EMPTY Env Variable ****")
            : Environment.GetEnvironmentVariable("AWS_PRODUCT_INTEGRATION_CORE");

        public ProductService(IGetProductsExternal productExternalService, IProductServiceAdapter productServiceAdapter, IAwsService awsService)
        {
            _productExternalService = productExternalService;
            _productServiceAdapter = productServiceAdapter;
            _awsService = awsService;
        }

        public async Task<IActionResult> GetItemList(Guid customerId, string accessToken, int shopId, DateTime dateCreatedFrom, DateTime dateCreatedTo, 
            long? customerPlanId, CancellationToken cancellationToken)
        {
            try
            {
                var listIdString = string.Empty;
                var nextPage = true;
                var pageSize = 10;
                var page = 0;

                while (nextPage)
                {
                    var listItem = await _productExternalService.GetItemList(accessToken, shopId, dateCreatedFrom, dateCreatedTo, cancellationToken, page, pageSize);

                    if (listItem.response == null || listItem.response.item == null || listItem.response.item.Count == 0)
                    {
                        return new BadRequestObjectResult("[Integration.Shopee][External][GetOrderList] A lista de produtos está vazia.");
                    }

                    listIdString = string.Join(", ", listItem.response.item.Select(item => item.item_id));

                    var message = new WebHookPost
                    {
                        accessToken = accessToken,
                        shopId = shopId,
                        customerId = customerId,
                        itemIds = listIdString,
                        hashIntegration = SHOPEE,
                        EntityType = ETypeEntity.Product,
                        customerPlanId = customerPlanId,
                        status = new Dictionary<string, string>()
                    };

                    var webHookContentMessage = JsonConvert.SerializeObject(message);

                    await _awsService.SendAwsMessage(webHookContentMessage, _awsProductDetails);

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

        public async Task<Contracts.DomainModels.Products.Product> GetItemDetail(WebHookPost webHookContentMessage)
        {
            var product = new Contracts.DomainModels.Products.Product();
            if (!string.IsNullOrEmpty(webHookContentMessage.itemIds))
            {
                var itemIds = webHookContentMessage.itemIds.Split(',');

                try
                {
                    foreach (var idProduct in itemIds)
                    {
                        var productDetailItem = await _productExternalService.GetItemDetail(webHookContentMessage.accessToken, webHookContentMessage.shopId, Convert.ToInt64(idProduct), new CancellationToken());

                        if (productDetailItem is { response: not null } &&
                            productDetailItem.response.item_list != null &&
                            productDetailItem.response.item_list.Count > 0)
                        {

                            var productVariationList = await _productExternalService.GetModelItemList(webHookContentMessage.accessToken, webHookContentMessage.shopId, Convert.ToInt64(idProduct), new CancellationToken());
                            var productAdapter = new ShopeeToProductAdapter(productDetailItem.response.item_list[0], _productExternalService, productVariationList, webHookContentMessage);
                            product = await _productServiceAdapter.ConverToProductAdapter(productAdapter);
                            product.CustomerId = webHookContentMessage.customerId;
                            product.CustomerPlanId = webHookContentMessage.customerPlanId;
                            var objectRequest = new ObjectRequest
                            {
                                EntityType = ETypeEntity.Product,
                                JsonObject = JsonConvert.SerializeObject(product),
                                HashIntegracao = SHOPEE
                            };

                            var json = JsonConvert.SerializeObject(objectRequest);

                            await _awsService.SendAwsMessage(json, _awsProductIntegration);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new Exception(e.Message);
                }
            }

            return product;
        }

    }
}
