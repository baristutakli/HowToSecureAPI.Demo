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
    //[ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users/")]
    //[Route("api/[controller]/")]
    //[Route("[controller]")]
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

        /// <summary>
        /// Azure AD Internal Policy
        /// Only Azure AD INTERNAL apps allowed
        /// This is created to show you azure ad internal authorization flow, only internal ad token will be validated and internal apps will be able to make request
        /// </summary>
        /// <returns></returns>
        [HttpGet("/acceptInternalADToken")]
        [Authorize(ApiConstants.InternalUserReadPolicyName)]
        public IActionResult AcceptInternalADToken()
        {
            return Ok(Users);
        }

        /// <summary>
        /// Only Azure AD INTERNAL And B2C apps allowed
        /// This is created to show you azure ad internal authorization flow, only internal ad and Azure AD B2C token will be validated and internal apps and Azure AD B2C apps will be able to make request
        /// </summary>
        /// <returns></returns>
        [HttpGet("/acceptInternalADTokenAndB2CToken")]
        [Authorize(ApiConstants.InternalAndB2CUserReadPolicyName)]
        public IActionResult AcceptInternalADTokenAndB2CToken()
        {
            return Ok(Users);
        }

        /// <summary>
        /// B2c apps and Azure Ad Internal
        /// B2c apps and Azure Ad Internal apps are allowed to make request to this endpoint
        /// This is created to show you B2c apps and Azure Ad Internal authorization flow, only internal ad token will be validated and internal apps will be able to make request
        /// </summary>
        /// <returns></returns>
        [HttpGet("/acceptmultipletoken")]
        [Authorize(ApiConstants.MultipleValidationUserReadPolicyName)]
        public IActionResult Acceptmultipletoken()
        {
            return Ok(Users);
        }
    }
}
