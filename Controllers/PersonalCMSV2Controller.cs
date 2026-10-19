using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personalCMSV2_NET_API.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace personalCMSV2_NET_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalCMSV2Controller : ControllerBase
    {
        public record CreateModelRequest([Required] string ModelName);
        public record CreateEntryRequest(
            [Required] string EntryName, 
            [Required] string ModelNameUuid
        );
        public record AddToModelRequest(
            [Required] string FieldName,
            [Required] string FieldType
        );
        public record EditEntryFields(
           [Required] ContentFieldsInterface Fields
       );

        private readonly AppDbContext _context;

        public PersonalCMSV2Controller(AppDbContext context)
        {
            _context = context;
        }

        // Helper method to convert a string to camelCase
        public static string ToCamelCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var words = Regex.Split(input.Trim(), @"[\s_\-]+")
                             .Where(w => w.Length > 0)
                             .ToArray();

            var ti = CultureInfo.InvariantCulture.TextInfo;

            return words[0].ToLowerInvariant() +
                   string.Concat(words.Skip(1).Select(w => ti.ToTitleCase(w.ToLowerInvariant())));
        }
        // Helper method to find a field in a JsonArray by its name
        public static Field? FindFieldExists(List<Field> fieldList, string fieldName)
        {
            return fieldList.FirstOrDefault(f =>
                    string.Equals(
                    f?.Id,
                    ToCamelCase(fieldName),
                    StringComparison.OrdinalIgnoreCase)
                    );
        }
        // Helper method to find a specific model by its UUID
        public ContentModel? FindSpecificModel(string uuid)
        {
            var model = _context.ContentModel
            .Where(c => c.Uuid == uuid)
            .Select(c => new ContentModel
            {
                Uuid = c.Uuid,
                EntryName = c.EntryName,
                Fields = c.Fields,
                CreatedAt = c.CreatedAt,
                LastUpdated = c.LastUpdated
            })
            .FirstOrDefault();
            return model;
        }
        private static ContentFieldsInterface? FieldStructureGenerator(List<Field> modelFieldStructure, string contentName)
        {
            if (modelFieldStructure == null) return null;

            var fields = new ContentFieldsInterface();

            foreach (var field in modelFieldStructure)
            {
                fields[field.Id] = new LocalizedField
                {
                    ["en-US"] = field.Id == "key" ? ToCamelCase(contentName) : ""
                };
            }

            return fields;
        }

        // Get all models
        [HttpGet("models")]
        public async Task<IActionResult> GetModels()
        {
            var result = await _context.ContentModel.Select(c => new
            {
                c.Uuid,
                c.EntryName,
                c.Fields,
                c.CreatedAt,
                c.LastUpdated
            }).ToListAsync();

            return Ok(result);
        }
        // Get a specific model by UUID
        [HttpGet("model/find/{uuid}")]
        public async Task<IActionResult> GetModel(string uuid)
        {
            var model = FindSpecificModel(uuid);
            if (model == null) return NotFound();
            return Ok(model);
        }
        
        
        // Create a new model
        [HttpPost("model/add")]
        public async Task<IActionResult> CreateModel([FromBody] CreateModelRequest userModelInput)
        {
            if (userModelInput.ModelName == null) return Conflict($"ModelName is needed.");
            var existing = await _context.ContentModel.FirstOrDefaultAsync(m => m.Uuid == ToCamelCase(userModelInput.ModelName));
            if (existing != null) return Conflict($"ModelName '{userModelInput.ModelName}' already exists.");
            var model = new ContentModel
            {
                Uuid = ToCamelCase(userModelInput.ModelName),
                EntryName = userModelInput.ModelName,
                Fields = new List<Field>(),
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _context.ContentModel.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        
        // Add a field to a model
        [HttpPost("model/field/{uuid}")]
        public async Task<IActionResult> AddModelField(string uuid, [FromBody] AddToModelRequest FieldToAdd)
        {
            if (uuid == null) return Conflict($"EntryName is needed.");
            var model = await _context.ContentModel.FirstOrDefaultAsync(m => m.Uuid == uuid);
            if (model != null)
            {
                List<Field> fieldList = model.Fields ?? new List<Field>();

                var exists = FindFieldExists(fieldList, FieldToAdd.FieldName);
                // Check if the field already exists in the model
                if (exists != null)
                    return Conflict($"A field with id '{FieldToAdd.FieldName}' already exists in this model.");
                fieldList.Add(new Field
                {
                    Id = ToCamelCase(FieldToAdd.FieldName),
                    Name = FieldToAdd.FieldName,
                    Type = FieldToAdd.FieldType,
                    Omitted = false,
                    Disabled = false,
                    Required = true,
                    Localized = false,
                    Validations = new List<FieldValidation>
                    {
                        new FieldValidation
                        {
                            Unique = true
                        }
                    }
                });
                model.Fields = fieldList;
                model.LastUpdated = DateTime.UtcNow;
                // Make sure to mark the Fields property as modified
                _context.ContentModel.Entry(model).Property(e => e.Fields).IsModified = true;
                // Then save the changes
                await _context.SaveChangesAsync();
                return Ok(model);
            }
             return Conflict($"EntryName '{uuid}' does exist.");
        }
        // Delete a field from a model
        [HttpDelete("model/field/{uuid}")]
        public async Task<IActionResult> DeleteModelField(string uuid, [FromBody] AddToModelRequest FieldToAdd)
        {
            if (uuid == null) return Conflict($"EntryName is needed.");
            var model = await _context.ContentModel.FirstOrDefaultAsync(m => m.Uuid == uuid);
            if (model != null)
            {
                List<Field> fieldList = model.Fields ?? new List<Field>();
                var exists = FindFieldExists(fieldList, FieldToAdd.FieldName);
                // Check if the field already exists in the model
                if (exists != null)
                { 
                   fieldList.Remove(exists);
                    model.Fields = fieldList;
                    model.LastUpdated = DateTime.UtcNow;
                   // Make sure to mark the Fields property as modified
                   _context.ContentModel.Entry(model).Property(e => e.Fields).IsModified = true;
                   // Then save the changes
                   await _context.SaveChangesAsync();
                   return Ok(model); 
                }
                
            }
            return Conflict($"FieldName '{FieldToAdd.FieldName}' was not found.");
        }
        // Get all entries
        [HttpGet("entries")]
        public async Task<IActionResult> GetEntries()
        {
            var result = await _context.ContentEntry.Select(c => new
            {
                c.Id,
                c.ModelUuid,
                c.Fields,
                c.CreatedAt,
                c.UpdatedAt,
                c.Name,
                c.ModelName
            }).ToListAsync();

            return Ok(result);
        }
        // Get a specific entry by Id
        [HttpGet("entry/{Id}")]
        public async Task<IActionResult> GetEntry(Guid Id)
        {
            var entry = await _context.ContentEntry
            .Where(c => c.Id == Id)
            .Select(c => new
            {
                c.Id,
                c.ModelUuid,
                c.Fields,
                c.CreatedAt,
                c.UpdatedAt,
                c.Name,
                c.ModelName
            })
            .FirstOrDefaultAsync();
            if (entry == null) return NotFound();
            return Ok(entry);
        }

        // Create a new entry
        [HttpPost("entry/add")]
        public async Task<IActionResult> CreateEntry([FromBody] CreateEntryRequest userEntryInput)
        {
            // Validate the input
            if (userEntryInput.ModelNameUuid == null && userEntryInput.EntryName == null) return Conflict($"ModelNameUuid and EntryName are needed.");
            if (userEntryInput.EntryName == null) return Conflict($"EntryName is needed.");
            if (userEntryInput.ModelNameUuid == null) return Conflict($"ModelNameUuid is needed.");
            var existing = await _context.ContentEntry.FirstOrDefaultAsync(e => e.Name == userEntryInput.EntryName);
            if (existing != null) return Conflict($"EntryName '{userEntryInput.EntryName}' already exists.");
            // Check if the model exists and retrieve it
            var model = FindSpecificModel(userEntryInput.ModelNameUuid);
            // If the model does not exist, return a conflict response
            if (model == null) return Conflict($"Model with UUID '{userEntryInput.ModelNameUuid}' was not found.");
            // Generate the field structure for the new entry based on the model's fields
            var fieldList = FieldStructureGenerator(model.Fields, userEntryInput.EntryName) ?? new ContentFieldsInterface();
            // Create a new ContentEntry object with the provided information and the generated field structure
            var entry = new ContentEntry
            {
                ModelUuid = model.Uuid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = userEntryInput.EntryName,
                ModelName = model.EntryName,
                Fields = fieldList
            };
            // Add the new entry to the database context and save changes
            _context.ContentEntry.Add(entry);
            await _context.SaveChangesAsync();
            return Ok(entry);
        }
        // Update an existing entry
        [HttpPut("entry/{Id}")]
        public async Task<IActionResult> UpdateEntry(Guid Id, [FromBody] EditEntryFields entry)
        {
            var existingEntry = await _context.ContentEntry.FindAsync(Id);
            if (existingEntry == null) return NotFound();
            existingEntry.Fields = entry.Fields;
            existingEntry.UpdatedAt = DateTime.UtcNow;
            _context.ContentEntry.Update(existingEntry);
            await _context.SaveChangesAsync();
            return Ok(existingEntry);
        }
    }
}
