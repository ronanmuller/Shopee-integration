using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Domain.Response.Products
{
    public class ProductCategoryResponse
    {
        public string error { get; set; }
        public string message { get; set; }
        public string warning { get; set; }
        public string request_id { get; set; }
        public CategoryResponse response { get; set; }
        public AuthUserResponseError? ErrorContent { get; set; }

    }

    public class CategoryList
    {
        public int category_id { get; set; }
        public int parent_category_id { get; set; }
        public string original_category_name { get; set; }
        public string display_category_name { get; set; }
        public bool has_children { get; set; }
    }

    public class CategoryResponse
    {
        public List<CategoryList> category_list { get; set; }
    }

}
