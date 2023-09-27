namespace xingyi.cas.common 
{
    using System.ComponentModel.DataAnnotations;

    public class ContentItem
    {
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
    }

}
