namespace GE.Integration.Shopee.Model
{
    public class ProductImport : BaseModel
    {
        public long Id { get; set; }
        public Guid CustomerId { get; set; }
        public long? CustomerPlanId { get; set; }
        public string ProductId { get; set; }
        public double? Cost { get; set; }
        public string Sku { get; set; }
    }
}
