using Cofidis.Credit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cofidis.Credit.Infrastructure.Configurations
{
    public static class DataLayerConfiguration
    {
        public static IServiceCollection ConfigureDataLayer(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var connectionString = environment.IsEnvironment("Test") ? configuration.GetConnectionString("CofidisCreditTest") :
                configuration.GetConnectionString("CofidisCredit");

            services.AddDbContext<CreditDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}
