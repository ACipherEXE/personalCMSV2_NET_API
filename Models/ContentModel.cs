using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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
        private List<Field> _fields = [];
        [Column("fields", TypeName = "jsonb")]
        public List<Field> Fields
        {
            get => _fields;
            set => _fields = value ?? [];
        }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

    }

}
public class Field

{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("omitted")]
    public bool Omitted { get; set; }

    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("localized")]
    public bool Localized { get; set; }

    [JsonPropertyName("validations")]
    public List<FieldValidation> Validations { get; set; } = new();
}
