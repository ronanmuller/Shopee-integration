using GE.Integration.Shopee.Model;

namespace GE.Integration.Shopee.Infra.Repositories
{
    public class OrderImportRepository : ARepository<OrderImport>
    {
        public OrderImportRepository(CoreDbContext context) : base(context) { }

    }
}
