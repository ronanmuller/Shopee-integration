using DevSnap.CommonLibrary.Extensions;
using GE.Core.Data.Mappings;
using GE.Integration.Shopee.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GE.Integration.Shopee.Infra
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
        }

        public DbSet<OrderImport> OrderImports { get; set; }
        public DbSet<ProductImport> ProductImports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.AddConfiguration(new OrderImportMap());
            builder.AddConfiguration(new ProductImportMap());
        }
    }
}
