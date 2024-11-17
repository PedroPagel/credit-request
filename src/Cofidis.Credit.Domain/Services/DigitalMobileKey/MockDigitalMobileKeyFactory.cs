using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MockDigitalMobileKey.Api;

namespace Cofidis.Credit.Infrastructure.DigitalMobileKeyFactory
{
    public class MockDigitalMobileKeyFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development"); 

            builder.ConfigureServices(services =>
            {
            });
        }
    }
}
