using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using UbicatApi.Models;

namespace UbicatApi
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<DataContext>();

            options.UseMySql(
                config.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection"))
            );

            return new DataContext(options.Options);
        }
    }
}
