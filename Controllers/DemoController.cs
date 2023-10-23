using HowToSecureAPI.Demo.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HowToSecureAPI.Demo.Controllers
{
    /// <summary>
    /// Demo controller
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/users/")]
    public class DemoController : ControllerBase
    {
        private static readonly string[] Users = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DemoController> _logger;

        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Azure AD B2C Policy
        ///  The prupose of this endpoint to show you how the policy and auth works
        /// </summary>
        /// <returns></returns>
        [HttpGet("/acceptB2CToken")]
        [Authorize(ApiConstants.UserReadPolicyName)]
        public IActionResult acceptB2CToken()
        {
            return Ok(Users);
        }
    }
}
