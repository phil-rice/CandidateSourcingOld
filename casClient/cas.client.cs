using System.Net.Http.Headers;
using xingyi.cas.common;
using xingyi.common;
using xingyi.common.http;

namespace xingyi.cas.client
{
    public interface ICasAdder
    {
        Task<string> AddItemAsync(string nameSpace, byte[] payload, string mimeType);
    }

    public interface ICasGetter
    {
        Task<ContentItem> GetItemAsync(string nameSpace, string sha);
    }


    public class CasClient : ICasAdder, ICasGetter
    {
        private readonly IHttpClient httpClient;
        private readonly IShaCodec shaCodec;

        public CasClient(IHttpClient httpClient, IShaCodec shaCodec)
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
    }


}