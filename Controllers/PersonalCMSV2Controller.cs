using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personalCMSV2_NET_API.Models;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace personalCMSV2_NET_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalCMSV2Controller : ControllerBase
    {
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
        private readonly AppDbContext _context;

        public PersonalCMSV2Controller(AppDbContext context)
        {
            _context = context;
        }

        // Model
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
        [HttpGet("model/{uuid}")]
        public async Task<IActionResult> GetModel(string uuid)
        {
            var model = await _context.ContentModel
            .Where(c => c.Uuid == uuid)
            .Select(c => new
            {
                c.Uuid,
                c.EntryName,
                c.Fields,
                c.CreatedAt,
                c.LastUpdated
            })
            .FirstOrDefaultAsync();
            if (model == null) return NotFound();
            return Ok(model);
        }
        [HttpPost("model")]
        public async Task<IActionResult> CreateModel([FromBody] ContentModel model)
        {
            _context.ContentModel.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);

        }
        // Entries
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
        [HttpGet("entries/{Id}")]
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
        [HttpPost("entrie")]
        public async Task<IActionResult> CreateEntry([FromBody] ContentEntry entry)
        {
            entry.Id = Guid.NewGuid();
            entry.CreatedAt = DateTime.UtcNow;
            entry.UpdatedAt = DateTime.UtcNow;
            _context.ContentEntry.Add(entry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntry), new { Id = entry.Id }, entry);

        }
        [HttpPut("entrie")]
        public async Task<IActionResult> UpdateEntry([FromBody] ContentEntry entry)
        {
            var existingEntry = await _context.ContentEntry.FindAsync(entry.Id);
            if (existingEntry == null) return NotFound();
            existingEntry.ModelUuid = entry.ModelUuid;
            existingEntry.Fields = entry.Fields;
            existingEntry.UpdatedAt = DateTime.UtcNow;
            existingEntry.Name = entry.Name;
            existingEntry.ModelName = entry.ModelName;
            _context.ContentEntry.Update(existingEntry);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
