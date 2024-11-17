using Cofidis.Credit.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cofidis.Credit.Tests.Integration
{
    public class IntegrationTestHelper
    {
        public HttpClient Client { get; private set; }
        public CodifidsCreditRequestWebApiAplicationFactory<Program> Factory { get; private set; }
        public static IntegrationTestHelper Instance { get; protected set; } = new IntegrationTestHelper();

        public IntegrationTestHelper()
        {
            Factory ??= new();

            Client ??= Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri(Factory.Url),
                AllowAutoRedirect = false
            });
        }
    }
}
