namespace xingyi.common
{
    using xingyi.common.validator;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public interface IShaCodec
    {
        string ComputeSha(byte[] content);
        bool Validate(byte[] content, string providedSha);

        IValidator<byte[]> validateAgainst(string providedSha, string message)
        {
            return IValidator<byte[]>.FromPredicate(body =>
            Validate(body, providedSha),
            body => message);
        }
    }

    public class ShaCodec : IShaCodec
    {
        public string ComputeSha(byte[] content)
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(content);

            // Convert the byte array into a URL-friendly base64 encoded string
            string base64 = Convert.ToBase64String(hashBytes);
            return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public bool Validate(byte[] content, string providedSha)
        {
            string computedSha = ComputeSha(content);
            return computedSha.Equals(providedSha, StringComparison.OrdinalIgnoreCase);
        }
    }

}
