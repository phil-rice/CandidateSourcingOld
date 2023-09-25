namespace CandidateSourcing.common
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public interface IShaCodex
    {
        string ComputeSha(byte[] content);
        bool Validate(byte[] content, string providedSha);
    }

    public class ShaCodex : IShaCodex
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
