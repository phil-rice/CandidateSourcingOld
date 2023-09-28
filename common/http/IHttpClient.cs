using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using xingyi.common.validator;

namespace xingyi.common.http
{
    public interface IHttpClient
    {
        Task<ErrorsAnd<HttpResponseMessage>> get(string uri);
        Task<ErrorsAnd<HttpResponseMessage>> post(string uri, string contentType, byte[] body);
    }

    public class DefaultHttpClient : IHttpClient
    {
        readonly HttpClient httpClient;

        public DefaultHttpClient(HttpClient httpClient, string baseUri)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(baseUri);
        }

        async public Task<ErrorsAnd<HttpResponseMessage>> get(string uri)
        {
            return await process(await httpClient.GetAsync(uri), $"Error getting from {uri}");
        }

        async public Task<ErrorsAnd<HttpResponseMessage>> post(string uri, string contentType, byte[] payload)
        {
            using var content = new ByteArrayContent(payload);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return await process(await httpClient.PostAsync(uri, content), $"Error posting to {uri}");
        }

        async Task<ErrorsAnd<HttpResponseMessage>> process(HttpResponseMessage response, string prefix)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return ErrorsAndMaker.value<HttpResponseMessage>(null);
            if (response.IsSuccessStatusCode)
                return ErrorsAndMaker.value<HttpResponseMessage>(response);

            var responseContent = await response.Content.ReadAsStringAsync();
            return ErrorsAndMaker.errors<HttpResponseMessage>(new List<string> { $"{prefix} {response.StatusCode}/{responseContent}" });
        }


    }
}
