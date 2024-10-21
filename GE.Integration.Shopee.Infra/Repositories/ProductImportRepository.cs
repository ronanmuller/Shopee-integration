using GE.Integration.Shopee.Model;

namespace GE.Integration.Shopee.Infra.Repositories
{
    public class ProductImportRepository : ARepository<ProductImport>
    {
        public ProductImportRepository(CoreDbContext context) : base(context) { }

    }
}
