namespace xingyi.cas.common
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using xingyi.common;

    public class ContentItem
    {
        public static ContentItem from(IShaCodec codec, string Namespace, string s, string MimeType = "application/json")
        {
            var data = Encoding.UTF8.GetBytes(s);
            return new ContentItem(Namespace, codec.ComputeSha(data), MimeType, data);
        }
        [Required]
        [MaxLength(256)] // Assuming a SHA-256 hash
        public string SHA { get; set; }

        [Required]
        [MaxLength(100)]
        public string Namespace { get; set; }

        [Required]
        [MaxLength(100)]
        public string MimeType { get; set; }

        [Required]
        public byte[] Data { get; set; }

        public ContentItem(string Namespace, string SHA, string MimeType, byte[] Data)
        {
            this.Namespace = Namespace;
            this.SHA = SHA;
            this.MimeType = MimeType;
            this.Data = Data;
        }
        public override bool Equals(object obj)
        {
            if (obj is ContentItem otherItem)
            {
                return SHA == otherItem.SHA &&
                       Namespace == otherItem.Namespace &&
                       MimeType == otherItem.MimeType &&
                       Enumerable.SequenceEqual(Data, otherItem.Data);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = SHA?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ (Namespace?.GetHashCode() ?? 0);
            hashCode = (hashCode * 397) ^ (MimeType?.GetHashCode() ?? 0);
            hashCode = (hashCode * 397) ^ (Data?.Sum(b => b) ?? 0); // Simplified approach for byte array, could be enhanced for larger data.
            return hashCode;
        }
    }
}
