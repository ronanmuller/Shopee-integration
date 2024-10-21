using GE.Integration.Shopee.Domain.Enum;

namespace GE.Integration.Shopee.Domain.Request
{
    public class ObjectRequest
    {
        public string JsonObject { get; set; }
        public ETypeEntity EntityType { get; set; }
        public string HashIntegracao { get; set; }
    }
}
