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


        [HttpGet("entries")]
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
    }
}
