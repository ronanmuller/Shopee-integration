using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GE.Integration.Shopee.Infra
{
    public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
    {
        public CoreDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            optionsBuilder.UseSqlServer(connectionString);
            
            return new CoreDbContext(optionsBuilder.Options);
        }
    }
}
