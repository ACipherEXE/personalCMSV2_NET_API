using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace personalCMSV2_NET_API.Models
{
    [Table("content_model")]
    public class ContentModel 
    {
        [Key]
        [Column("uuid")]
        public string Uuid { get; set; } = default!;
        [Column("entry_name")]
        public string EntryName { get; set; } = default!;
        [Column("fields")]
        public JsonElement Fields { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

    }
}
