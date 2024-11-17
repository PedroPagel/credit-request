using Cofidis.Credit.Domain.Models.DigitalMobileKey;
using Cofidis.Credit.Infrastructure.DigitalMobileKeyFactory;
using System.Text.Json;

namespace Cofidis.Credit.Domain.Services.DigitalMobileKey
{
    public class DigitalMobileKeyService : IDigitalMobileKeyService
    {
        private readonly HttpClient _client;

        public DigitalMobileKeyService()
        {
            var factory = new MockDigitalMobileKeyFactory();
            _client = factory.CreateClient();
        }

        public async Task<UserInfo> GetUserInfoByNIF(string nif)
        {
            var response = await _client.GetAsync($"/api/digitalmobilekey/{nif}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var userInfo = JsonSerializer.Deserialize<UserInfo>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return userInfo;
        }
    }
}
