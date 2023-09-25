namespace CandidateSourcing.common
{
    public static class RequestHelper
    {

        public static async Task<byte[]> BodyToByteArray(HttpRequest request)
        {
            using var memoryStream = new MemoryStream();
            await request.Body.CopyToAsync(memoryStream);
            byte[] content = memoryStream.ToArray();
            return content;

        }
    }
}
