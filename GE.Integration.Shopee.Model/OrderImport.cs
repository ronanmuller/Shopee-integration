namespace GE.Integration.Shopee.Model
{
    public class OrderImport : BaseModel
    {
        public long Id { get; set; }
        public Guid? CustomerId { get; set; }
        public long? CustomerPlanId { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string EcommerceNumber { get; set; }
        public string? SalesChannel { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
