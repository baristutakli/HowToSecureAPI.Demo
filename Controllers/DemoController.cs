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
        /// Azure AD INTERNAL And Azure AD B2C apps allowed
        /// This is created to show you Azure AD INTERNAL And Azure AD B2C authorization flow, only internal ad and Azure AD B2C token will be validated and internal apps and Azure AD B2C apps will be able to make request
        /// </summary>
        /// <returns></returns>
        [HttpGet("/acceptInternalADTokenAndB2CToken")]
        [Authorize(ApiConstants.InternalAndB2CUserReadPolicyName)]
        public IActionResult AcceptInternalADTokenAndB2CToken()
        {
            return Ok(Users);
        }

    }
}
