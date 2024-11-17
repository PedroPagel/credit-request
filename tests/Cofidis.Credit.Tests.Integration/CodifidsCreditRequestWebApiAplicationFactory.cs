using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Cofidis.Credit.Tests.Integration
{
    public class CodifidsCreditRequestWebApiAplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly IConfigurationRoot _configurationRoot;
        public string Url => _configurationRoot["applicationUrl"];

        public CodifidsCreditRequestWebApiAplicationFactory()
        {
            _configurationRoot = new ConfigurationBuilder()
               .AddJsonFile("appsettings.tests.json")
               .AddEnvironmentVariables()
               .Build();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.Build();
                config.AddJsonFile("appsettings.json");
                config.AddEnvironmentVariables();
            });

            builder.UseEnvironment("Test");

            return base.CreateHost(builder);
        }
    }
}
