using DevSnap.CommonLibrary.Extensions;
using GE.Integration.Shopee.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GE.Core.Data.Mappings
{
    internal class ProductImportMap : EntityTypeConfiguration<ProductImport>
    {
        public override void Map(EntityTypeBuilder<ProductImport> builder)
        {
            builder.ToTable("ProductImports");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Sku).HasMaxLength(200);
            builder.Property(x => x.ProductId).HasMaxLength(200);
        }
    }
}
