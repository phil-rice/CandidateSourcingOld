using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using xingyi.cas.common;
using xingyi.common;
using xingyi.common.http;

namespace xingyi.cas.client
{
    public interface ICasAdder
    {
        Task<string> AddItemAsync(string nameSpace, byte[] payload, string mimeType);
    }

    public interface ICasJsonGetter
    {
        Task<Dictionary<string, object>> GetJsonAsync(string nameSpace, string sha);

    }
    public interface ICasGetter
    {
        Task<ContentItem> GetItemAsync(string nameSpace, string sha);
    }


    public class CasClient : ICasAdder, ICasGetter, ICasJsonGetter
    {
        private readonly IHttpClient httpClient;
        private readonly IShaCodec shaCodec;

        public static string HttpClientName = "casClient";
        public CasClient(IHttpClientFactory factory, IShaCodec shaCodec) // for config
        {
            this.httpClient = new DefaultHttpClient(factory.CreateClient(HttpClientName));
            this.shaCodec = shaCodec;
        }

        public static CasClient forTests(IHttpClient httpClient, IShaCodec shaCodec)
        {
            return new CasClient(httpClient, shaCodec);
        }
        private CasClient(IHttpClient httpClient, IShaCodec shaCodec) // for tests
        {
            this.httpClient = httpClient;
            this.shaCodec = shaCodec;
        }

        public async Task<string> AddItemAsync(string nameSpace, byte[] payload, string mimeType)
        {
            var response = (await httpClient.post($"/cas/{nameSpace}", mimeType, payload)).valueOrError();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<ContentItem> GetItemAsync(string nameSpace, string sha)
        {
            var response = (await httpClient.get($"/cas/{nameSpace}/content/{sha}")).valueOrError();
            if (response == null) return null;
            var MimeType = response.Content.Headers?.ContentType?.ToString() ?? "";
            var data = await response.Content.ReadAsByteArrayAsync();
            var checkedSha = shaCodec.ComputeSha(data);
            if (checkedSha != sha) throw new ShaMismatchException(sha, checkedSha, data);
            return new ContentItem(nameSpace, sha, MimeType, data);
        }

        public async Task<Dictionary<string, object>> GetJsonAsync(string nameSpace, string sha)
        {
            var contentItem = await GetItemAsync(nameSpace, sha);
            if (contentItem == null) throw new CasNotFoundException(nameSpace, sha);
            if (!contentItem.MimeType.Contains("json")) throw new CasNotJsonException(contentItem);
            string jsonString = System.Text.Encoding.UTF8.GetString(contentItem.Data);
            Dictionary<string, object> result = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
            return result;
        }
    }


}