using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace personalCMSV2_NET_API.Models
{
    [Table("content_entry")]
    public class ContentEntry
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = default!;
        [Column("model_uuid")]
        public string ModelUuid { get; set; } = default!;
        [Column("fields")]
        public JsonElement Fields { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("name")]
        public string Name { get; set; } = default!;
        [Column("model_name")]
        public string ModelName { get; set; } = default!;

    }
}

public class LocalizedValue
{
    [JsonPropertyName("en_US")]
    public string EnUS { get; set; } = "";
}

public class FieldValidation
{
    [JsonPropertyName("unique")]
    public bool? Unique { get; set; }
}