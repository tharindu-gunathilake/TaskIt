using Microsoft.AspNetCore.Mvc;
using TaskIt.Data;

namespace TaskIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly TaskItDbContext _dbContext;

        public HealthController(TaskItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Health()
        {
            try
            {
                await _dbContext.Database.CanConnectAsync();

                return Ok(new
                {
                    status = "Healthy",
                    database = "Reachable",
                    timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "Unhealthy",
                    database = "Unreachable",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
