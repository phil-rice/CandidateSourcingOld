using System.ComponentModel.DataAnnotations;

namespace xingyi.events
{
    public class StoredEvent
    {
        [Required]
        [MaxLength(100)]
        public string Namespace { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string JsonEvents { get; set; }
    }
}
