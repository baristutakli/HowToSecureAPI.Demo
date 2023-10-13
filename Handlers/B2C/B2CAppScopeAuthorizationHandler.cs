using HowToSecureAPI.Demo.Constants;
using HowToSecureAPI.Demo.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HowToSecureAPI.Demo.Handlers.B2C
{
    public class B2CAppScopeAuthorizationHandler : AuthorizationHandler<B2CAppScopeRequirement>
    {
        private const int ReturnedScopesSizeByAzure = 1;
        /// <summary>
        /// CTOR
        /// </summary>
        public B2CAppScopeAuthorizationHandler()
        {
            // Inject the followings if you want to trace the flow
            // inject logger 
            // inject metric agent
        }

        /// <summary>
        /// Handle authorization
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, B2CAppScopeRequirement requirement)
        {

            if (requirement.Scopes == null) throw new ArgumentNullException(nameof(requirement.Scopes));

            var scopeIntersect = new List<string>();

            if (requirement.Scopes.Count != 0)
            {
                List<string> scopeClaim = new List<string>();

                // Try to get the incoming requests scopes
                if (context?.User?.FindFirst(ApiConstants.ApplicationScopeClaimIdentifier)?.Value != null)
                    scopeClaim = context?.User?.FindFirst(ApiConstants.ApplicationScopeClaimIdentifier)?.Value?.Split(" ")?.ToList();

                // We check whther the incoming requests scopes match with the required scopes
                scopeIntersect = requirement.Scopes.Intersect(scopeClaim).ToList();

                if (!scopeIntersect.Any())
                    return; // this will return unauthorized if the requests does not have right roles or scopes

            }

            context.Succeed(requirement);
        }

    }
}
