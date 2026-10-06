using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personalCMSV2_NET_API.Models;

namespace personalCMSV2_NET_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalCMSV2Controller : ControllerBase
    {
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
    }
}
