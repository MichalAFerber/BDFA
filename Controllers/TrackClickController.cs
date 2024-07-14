using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackClickController : ControllerBase
    {
        private readonly DirectoryContext _context;

        public TrackClickController(DirectoryContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> TrackClick([FromBody] ClickData clickData)
        {
            if (clickData == null || string.IsNullOrEmpty(clickData.Link) || string.IsNullOrEmpty(clickData.ClickDateTime.ToString()))
            {
                return BadRequest("Invalid data.");
            }

            await SaveClickDataAsync(clickData);

            return Ok(new { success = true });
        }

        private async Task SaveClickDataAsync(ClickData clickData)
        {
            // Fetch the Profile object from the database
            var click = await _context.Clicks.FirstOrDefaultAsync();

            // Log the result of the query
            if (click == null)
            {
            }
            else
            {
                click.ProfileId = clickData.ProfileId;
                click.Link = clickData.Link;
                click.ClickDateTime = clickData.ClickDateTime;

                // Mark the Profile entity as modified
                _context.Attach(click).State = EntityState.Modified;

                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
        }
    }
}
