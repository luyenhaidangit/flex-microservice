using DbUp;
using DbUp.Oracle;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;

namespace Flex.Securities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CoreController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("migration-database")]
        public IActionResult MigrationDatabase()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest(Result.Failure("Connection string is missing or invalid."));
            }

            var upgrader = DeployChanges.To
            .OracleDatabaseWithDefaultDelimiter("")
            .WithScriptsFromFileSystem(Path.Combine(AppContext.BaseDirectory, "Migrations/Scripts"))
            .LogToConsole()
            .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                return StatusCode(500, Result.Failure("Database migration failed: " + result.Error.ToString()));
            }


            return Ok(Result.Success());
        }
    }
}
