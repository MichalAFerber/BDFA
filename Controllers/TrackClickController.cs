using BDFA.Data;
using BDFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDFA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackClickController(DirectoryContext context, ILogger<TrackClickController> logger) : ControllerBase
    {
        private readonly DirectoryContext _context = context;
        private readonly ILogger<TrackClickController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> TrackClick([FromBody] ClickData clickData)
        {
            // Validate the incoming click data
            if (clickData == null || string.IsNullOrEmpty(clickData.Link) || string.IsNullOrEmpty(clickData.ClickDateTime.ToString()))
            {
                Console.WriteLine("Invalid data received.");
                _logger.LogWarning("Invalid data received.");
                return BadRequest("Invalid data received.");
            }

            // Save the click data asynchronously
            await SaveClickDataAsync(clickData);

            return Ok(new { success = true });
        }

        private async Task SaveClickDataAsync(ClickData clickData)
        {
            try
            {
                // Fetch the first ClickData object from the database
                var click = await _context.Clicks.FirstOrDefaultAsync();

                // Log the result of the query
                if (click == null)
                {
                    Console.WriteLine("No click data found in the database.");
                    _logger.LogInformation("No click data found in the database.");
                }
                else
                {
                    Console.WriteLine("Click data found. Updating the record.");
                    _logger.LogInformation("Click data found. Updating the record.");

                    // Update the click data with the new values
                    click.ProfileId = clickData.ProfileId;
                    click.Link = clickData.Link;
                    click.ClickDateTime = clickData.ClickDateTime;

                    // Mark the ClickData entity as modified
                    _context.Attach(click).State = EntityState.Modified;
                }

                // Save the changes to the database
                await _context.SaveChangesAsync();

                Console.WriteLine("Click data updated successfully.");
                _logger.LogInformation("Click data updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while saving the click data.");
                _logger.LogError(ex, "An error occurred while saving the click data.");
                throw; // Re-throw the exception to ensure it propagates
            }
        }
    }
}
