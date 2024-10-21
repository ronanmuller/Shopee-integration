using System.Globalization;
using GE.Contracts.DomainModels.Interfaces;
using GE.Contracts.DomainModels.Products;
using GE.Integration.Shopee.Domain.Response.Products;
using GE.Integration.Shopee.Domain.Response.WebHook;
using GE.Integration.Shopee.Infra.External.Products;

namespace GE.Integration.Shopee.Application.Adapters
{
    public class ShopeeToProductAdapter : IProductAdapter
    {
        private readonly ItemList _ProductResponse;
        private readonly IGetProductsExternal _getProductsExternal;
        private readonly ProductModelListResponse _productModelItem;
        private readonly int ShopId;
        private readonly string AccessToken;
        private bool productHasChildren;
        private int categoryId;

        public ShopeeToProductAdapter(ItemList item,
            IGetProductsExternal getProductsExternal,
            ProductModelListResponse productModelsItem,
            WebHookPost shopContentInformation)
        {
            _ProductResponse = item;
            _getProductsExternal = getProductsExternal;
            _productModelItem = productModelsItem;
            AccessToken = shopContentInformation.accessToken;
            ShopId = shopContentInformation.shopId;
        }

        public string InternalId() => _ProductResponse.item_id.ToString();

        public string Name() => _ProductResponse.item_name;

        public string Description() => _ProductResponse.description;

        public int IntegrationType() => 3;

        public int Variation() => 0;

        public string Category()
        {
            categoryId = _ProductResponse.category_id;

            var listCategories = _getProductsExternal.GetCategory(AccessToken, ShopId, new CancellationToken()).Result;

            if (listCategories == null ||
                listCategories.response == null ||
                !listCategories.response.category_list.Any())
                return "Not Found";

            var categoryList = listCategories.response.category_list.ToList();
            var categoryDescription = categoryList.FirstOrDefault(c => c.category_id == categoryId);

            if (categoryDescription is null)
                return "Not Found";

            productHasChildren = categoryDescription.has_children;
            return categoryDescription.display_category_name;
        }

        public string Brand() => _ProductResponse.brand.original_brand_name;

        public decimal Price()
        {
            if (_ProductResponse.price_info is { Count: > 0 })
                return (decimal)_ProductResponse.price_info[0].current_price;

            return 0;
        }

        public decimal Cost()
        {
            if (_ProductResponse.price_info is { Count: > 0 })
                return (decimal)_ProductResponse.price_info[0].original_price;

            return 0;
        }

        public int Stock()
        {
            if (_ProductResponse.stock_info is { Count: > 0 })
                return (int)_ProductResponse.stock_info[0].current_stock;

            return 0;
        }

        public int TotalSales()
        {
            var listExtraInfo = _getProductsExternal.GetItemExtraInfo(AccessToken, ShopId, new CancellationToken(), _ProductResponse.item_id).Result;

            if (listExtraInfo == null ||
                listExtraInfo.response == null ||
                !listExtraInfo.response.item_list.Any())
                return 0;

            var list = listExtraInfo.response.item_list.ToList();
            var listSales = list.FirstOrDefault(c => c.item_id == _ProductResponse.item_id);

            return listSales?.sale ?? 0;
        }

        public string Status() => _ProductResponse.item_status;

        public string SKU() => _ProductResponse.item_sku;

        public string GTIN() => _ProductResponse.gtin_code;

        public string? NCM() => _ProductResponse.tax_info?.ncm;

        public string Weight() => _ProductResponse.weight == null ? "0" : _ProductResponse.weight.Value.ToString(CultureInfo.InvariantCulture);

        public string? Height() => _ProductResponse.dimension == null ? "0" : _ProductResponse.dimension.package_height.ToString();

        public string? Length() => _ProductResponse.dimension == null ? "0" : _ProductResponse.dimension.package_length.ToString();

        public string? Width() => _ProductResponse.dimension == null ? "0" : _ProductResponse.dimension.package_width.ToString();

        public string TypeVariation()
        {
            if (productHasChildren)
                return "P";

            return "N";
        }

        public string Dimensions() => string.Empty;

        public string Color() => string.Empty;

        public List<Variation> Variations()
        {
            if (_productModelItem == null ||
                _productModelItem.Response == null ||
                _productModelItem.Response.model == null)
                return new List<Variation>();

            return _productModelItem.Response.model
                .Select(model => new Variation
                {
                    IdDoProduto = _ProductResponse.item_id.ToString(),
                    Sku = model.model_sku,
                    GtinEan = model.gtin_code,
                    Brand = _ProductResponse.brand?.original_brand_name,
                    Ncm = _ProductResponse.tax_info?.ncm,
                    Height = _ProductResponse.dimension == null ? 0 : _ProductResponse.dimension.package_height,
                    Width = _ProductResponse.dimension == null ? 0 : _ProductResponse.dimension.package_width,
                    Length = _ProductResponse.dimension == null ? 0 : _ProductResponse.dimension.package_length,
                    Weight = (float?)(_ProductResponse.weight == null ? 0 : Convert.ToDouble(_ProductResponse.weight)),
                    UpdatedAt = DateTime.Today,
                    CreatedAt = DateTime.Today,
                    TypeVariation = "V",
                    ReferenceVariationId = model.model_id.ToString()

                }).ToList();
        }

        public List<Picture> Pictures()
        {
            var pictures = new List<Picture>();
            var imageList = _ProductResponse.image;

            foreach (var imageUrl in imageList.image_url_list)
            {
                var picture = new Picture
                {
                    Url = imageUrl,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                pictures.Add(picture);

            }

            return pictures;
        }

        public List<Stock> Stocks()
        {
            var stockList = new List<Stock>();

            if (Variations().Count > 0)
            {
                foreach (var model in _productModelItem.Response.model)
                {
                    var stock = new Stock
                    {
                        IsActive = true,
                        Name = model.stock_info_v2.seller_stock == null ||
                               model.stock_info_v2.seller_stock.Count == 0 ? "Geral" : model.stock_info_v2.seller_stock[0].location_id,
                        Quantity = model.stock_info_v2.summary_info == null ? 0 : model.stock_info_v2.summary_info.total_available_stock,
                        ProductExternalId = _ProductResponse.item_id.ToString(),
                    };

                    stockList.Add(stock);
                }
            }
            else
            {
                var stockListInfo = _ProductResponse.stock_info;

                if (stockListInfo != null)
                    foreach (var s in stockListInfo)
                    {
                        var stock = new Stock
                        {
                            IsActive = true,
                            Name = s.stock_location_id,
                            Quantity = s.current_stock,
                            ProductExternalId = _ProductResponse.item_id.ToString()
                        };
                        stockList.Add(stock);
                    }
            }

            return stockList;
        }

        public int Quantity() => 0;

    }

}
