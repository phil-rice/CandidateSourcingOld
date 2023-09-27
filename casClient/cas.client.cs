using System.Net.Http.Headers;
using xingyi.common;

namespace xingyi.cas.client
{
    public interface ICasAdder
    {
        Task<string> AddItemAsync(string nameSpace,byte[] payload, string mimeType);
    }

    public interface ICasGetter
    {
        Task<byte[]> GetItemAsync(string nameSpace,string sha);
    }
    public class CasClient : ICasAdder, ICasGetter
    {
        private readonly HttpClient _httpClient;
        private readonly IShaCodec shaCodec;

        public CasClient(HttpClient httpClient, IShaCodec shaCodec,string host)
        {
            _httpClient = httpClient;
            this.shaCodec = shaCodec;
            _httpClient.BaseAddress = new Uri(host);
        }

        public async Task<string> AddItemAsync(string nameSpace,byte[] payload, string mimeType)
        {
            using var content = new ByteArrayContent(payload);
            content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            var response = await _httpClient.PostAsync($"/cas/{nameSpace}", content);

            if (!response.IsSuccessStatusCode)
            {
                var responseConent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error adding item: {nameSpace} {response.StatusCode}/{responseConent}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<byte[]> GetItemAsync(string nameSpace,string sha)
        {
            var response = await _httpClient.GetAsync($"/case/{nameSpace}/content/{sha}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error retrieving item {nameSpace}/${sha}: {response.StatusCode}/{response.Content.ToString}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }


}