using xingyi.cas.common;
using xingyi.common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using xingyi.common.validator;
using Microsoft.AspNetCore.Mvc;

namespace casApi
{
    [ApiController]
    [Route("cas")]
    public class CasController : ControllerBase
    {
        private readonly ICasRepository casRepo;
        private readonly IShaCodec shaCodec;

        private readonly IExtract<HttpRequest, string> extractContentType = Extract.fromHeader("Content-Type");


        public CasController(ICasRepository casRepo, IShaCodec shaCodec)
        {
            this.casRepo = casRepo;
            this.shaCodec = shaCodec;
        }

        [HttpPost("{namespace}")]
        public async Task AddItem(string @namespace)
        {
            byte[] content = await RequestHelper.BodyToByteArray(Request);
            string computedSha = shaCodec.ComputeSha(content);
            var existing = await casRepo.ContentItem(@namespace, computedSha);
            var path = Request.Path;
            ErrorsAnd<ContentItem> contentItemOrErrors = await extractContentType.extract(Request).flatMapK(async contentType =>
                await (existing == null
                ? processNewItem(new ContentItem(SHA: computedSha, Namespace: @namespace, MimeType: contentType, Data: content))
                : processExistingItem(existing, computedSha, content)));

            Response.ContentType = "text/plain; charset=utf-8";

            await contentItemOrErrors.forHttpResponseString(Response, HappyStatusCode: existing==null?201:200, result: ci =>$"{path}/content/{ci.SHA}");
        }

        private async Task<ErrorsAnd<ContentItem>> processNewItem(ContentItem contentItem)
        {
            await casRepo.Àdd(contentItem);
            return ErrorsAndMaker.value<ContentItem>(contentItem);
        }

        private async Task<ErrorsAnd<ContentItem>> processExistingItem(ContentItem existing, string computedSha, byte[] content)
        {
            return ErrorsAndMaker.errorsOr<ContentItem>(shaCodec.validateAgainst(computedSha, "Sha collision").Validate(existing.Data), existing);
        }


        [HttpGet("{nameSpace}/content/{sha}")]
        public async Task<IActionResult> GetContent(string nameSpace, string sha)
        {
            var contentItem = await casRepo.ContentItem(nameSpace, sha);
            if (contentItem == null) return NotFound("Content not found");

            Response.Headers.Add("MimeType", contentItem.MimeType);
            return File(contentItem.Data, contentItem.MimeType);
        }

        // GET samples
        [HttpGet("{nameSpace}/samples")]
        public IActionResult GetSamples(string nameSpace)
        {
            return Ok(casRepo.ContentItems(nameSpace));
        }
    }

}
