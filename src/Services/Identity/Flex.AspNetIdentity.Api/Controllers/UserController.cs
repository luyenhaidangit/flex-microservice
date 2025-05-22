using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Services.Interfaces;
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
    }
}
