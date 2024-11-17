using Cofidis.Credit.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Web;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Cofidis.Credit.Tests.Integration
{
    public class ServerFixture
    {
        private static HttpClient _client;
        private static CodifidsCreditRequestWebApiAplicationFactory<Program> _factory;

        public ServerFixture()
        {
            _factory = IntegrationTestHelper.Instance.Factory;
            _client = IntegrationTestHelper.Instance.Client;
        }

        public static async Task<T> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(url);

            return await GetResponseMessage<T>(response);
        }

        public static async Task<T> PatchAsync<T>(string url, object request = null)
        {
            var response = await _client.PatchAsync(url, GetContent(request));

            return await GetResponseMessage<T>(response);
        }

        public static async Task<T> PostAsync<T>(string url, object request = null)
        {
            var response = await _client.PostAsync(url, GetContent(request));

            return await GetResponseMessage<T>(response);
        }

        public static async Task<T> DeleteAsync<T>(string url)
        {
            var response = await _client.DeleteAsync(url);

            return await GetResponseMessage<T>(response);
        }

        private static StringContent GetContent(object request)
        {
            if (request == null)
                return null;

            var jsonRequest = JsonConvert.SerializeObject(request);
            return new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
        }

        private static async Task<T> GetResponseMessage<T>(HttpResponseMessage httpResponse)
        {
            if (HttpStatusCode.OK != httpResponse.StatusCode)
                return await Task.FromResult(default(T));

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseContent);

            return (jObject.ContainsKey("data") ? jObject["data"].ToObject<T>() : Task.FromResult(default(T)).Result);
        }
    }
}
