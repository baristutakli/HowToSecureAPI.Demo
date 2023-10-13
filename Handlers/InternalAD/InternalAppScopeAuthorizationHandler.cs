using HowToSecureAPI.Demo.Constants;
using HowToSecureAPI.Demo.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HowToSecureAPI.Demo.Handlers.InternalAD
{

    /// <summary>
    /// This will return unauthorized if the requests does not have right roles or scopes
    /// if there are missing scopes, this handler will return forbidden
    /// </summary>
    public class InternalAppScopeAuthorizationHandler : AuthorizationHandler<InternalAppScopeRequirement>
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public InternalAppScopeAuthorizationHandler()
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
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, InternalAppScopeRequirement requirement)
        {

            if (requirement.Scopes == null) throw new ArgumentNullException(nameof(requirement.Scopes));

            var scopeIntersect = new List<string>();

            if (requirement.Scopes.Count != 0)
            {
                List<string> scopeClaim = new List<string>();
                // Azure AD internal application role checks
                if (context?.User?.FindAll(ApiConstants.AzureADInternalApplicationRoleClaimİdentifier)?.Select(x => x.Value)?.ToList() != null)
                    scopeClaim = context.User.FindAll(ApiConstants.AzureADInternalApplicationRoleClaimİdentifier).Select(x => x.Value).ToList();

                // We check whther the incoming requests scopes match with the required scopes
                scopeIntersect = requirement.Scopes.Intersect(scopeClaim).ToList();

                if (!scopeIntersect.Any())
                    return;

            }

            context.Succeed(requirement);
        }


    }
}
