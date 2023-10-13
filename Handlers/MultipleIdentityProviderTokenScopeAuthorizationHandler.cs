using HowToSecureAPI.Demo.Configs.MultipleValidation.AzureADAndB2CWithOkta;
using HowToSecureAPI.Demo.Constants;
using HowToSecureAPI.Demo.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HowToSecureAPI.Demo.Handlers
{
    public class MultipleIdentityProviderTokenScopeAuthorizationHandler : AuthorizationHandler<MultipleIdentityProviderScopeRequirement>
    {
        private readonly MultipleValidationConfig _multipleValidationConfig;
        /// <summary>
        /// Multiple Identity Provider Token Scope Authorization Handler CTOR for Okta, Azure AD internal and Azure AD B2C
        /// </summary>
        public MultipleIdentityProviderTokenScopeAuthorizationHandler(MultipleValidationConfig multipleValidationConfig)
        {
            // Inject the followings if you want to trace the flow
            // inject logger 
            // inject metric agent
            _multipleValidationConfig = multipleValidationConfig;
        }

        /// <summary>
        /// Handle authorization
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MultipleIdentityProviderScopeRequirement requirement)
        {

            if (requirement.Scopes == null) throw new ArgumentNullException(nameof(requirement.Scopes));

            var scopeIntersect = new List<string>();

            if (requirement.Scopes.Count != 0)
            {

                List<string> scopeClaim = new List<string>();
                string? issuer = context?.User?.FindFirst(ApiConstants.ApplicationIssuerClaim)?.Value;
                if (!string.IsNullOrWhiteSpace(issuer))
                {
                    // B2C
                    if (_multipleValidationConfig.AzureADB2C.ValidIssuers.Contains(issuer))
                    {
                        if (context?.User?.FindFirst(ApiConstants.ApplicationScopeClaimIdentifier)?.Value != null)
                            scopeClaim = context?.User?.FindFirst(ApiConstants.ApplicationScopeClaimIdentifier)?.Value?.Split(" ")?.ToList();
                    }
                    // internal AD
                    else if (_multipleValidationConfig.AzureAD.ValidIssuers.Contains(issuer))
                    {
                        if (context?.User?.FindFirst(ApiConstants.AzureADInternalApplicationRoleClaimİdentifier)?.Value != null)
                            scopeClaim = context?.User?.FindAll(ApiConstants.AzureADInternalApplicationRoleClaimİdentifier)?.Select(x => x.Value)?.ToList();
                    }
                    // Okta
                    else if (_multipleValidationConfig.Okta.ValidIssuers.Contains(issuer))
                    {
                        if (context?.User?.FindFirst(ApiConstants.ApplicationScopeClaimIdentifier)?.Value != null)
                            scopeClaim = context?.User?.FindAll(ApiConstants.ApplicationScopeClaimIdentifier)?.Select(x => x.Value)?.ToList();
                    }
                    // We check whther the incoming requests scopes match with the required scopes
                    scopeIntersect = requirement.Scopes.Intersect(scopeClaim).ToList();

                    if (!scopeIntersect.Any())
                        return; // this will return unauthorized if the requests does not have right roles or scopes
                }
                else
                    return;

            }

            context.Succeed(requirement);
        }


    }
}
