using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.Requests;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    { 
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IBranchService _branchService;

        public UserController(UserManager<User> userManager, ILogger<AuthController> logger, IBranchService branchService)
        {
            _userManager = userManager;
            _logger = logger;
            _branchService = branchService;
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            _logger.LogInformation("Attempting to register user: {Email}", request.Email);

            // Validate
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(Result.Failure(message: "User already exists with this email."));
            }

            var existingUsername = await _userManager.FindByNameAsync(request.Email);
            if (existingUsername != null)
            {
                return BadRequest(Result.Failure(message: "User already exists with this username."));
            }

            var existingBranchId = await _branchService.IsBranchExistsAsync(request.BranchId);
            if (existingBranchId)
            {
                return BadRequest(Result.Failure(message: "Branch Id exists with this username."));
            }

            return Ok("ff");

            // Create user
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                BranchId = request.BranchId,
                FullName = $"{request.FirstName} {request.LastName}"
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(Result.Failure(result.Errors.First().Description));
            }

            return Ok(Result.Success());
        }
    }
}
