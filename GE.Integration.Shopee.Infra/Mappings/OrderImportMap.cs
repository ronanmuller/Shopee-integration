using DevSnap.CommonLibrary.Extensions;
using GE.Integration.Shopee.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GE.Core.Data.Mappings
{
    internal class OrderImportMap : EntityTypeConfiguration<OrderImport>
    {
        public override void Map(EntityTypeBuilder<OrderImport> builder)
        {
            builder.ToTable("OrderImports");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Status).HasMaxLength(200);
            builder.Property(x => x.EcommerceNumber).HasMaxLength(200);
            builder.Property(x => x.SalesChannel).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.OrderId).HasMaxLength(200);

        }
    }
}
