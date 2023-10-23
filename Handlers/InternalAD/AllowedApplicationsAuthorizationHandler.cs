using HowToSecureAPI.Demo.Configs.OnlyAzureADInternalValidation;
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
    /// Check whether an application is allowed to make request to this api or not
    /// </summary>
    public class InternalADAllowedApplicationsAuthorizationHandler : AuthorizationHandler<AllowedApplicationsRequirement>
    {
        private readonly AzureADValidationConfig _azureADValidationConfig;
        /// <summary>
        /// CTOR
        /// </summary>
        public InternalADAllowedApplicationsAuthorizationHandler(AzureADValidationConfig azureADValidationConfig)
        {
            // Inject the followings if you want to trace the flow
            // inject logger 
            // inject metric agent
            _azureADValidationConfig = azureADValidationConfig;
        }

        /// <summary>
        /// Handle authorization
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowedApplicationsRequirement requirement)
        {

            if (requirement.AppIds == null) throw new ArgumentNullException(nameof(requirement.AppIds));

            var appIdsIntersect = new List<string>();

            if (requirement.AppIds.Count != 0)
            {
                List<string> applicationIds = new List<string>();

                string? issuer = context?.User?.FindFirst(ApiConstants.ApplicationIssuerClaim)?.Value;
                string? appId = context?.User?.FindFirst(ApiConstants.AzureADInternalApplicationIdClaimİdentifier)?.Value;
                if (!string.IsNullOrWhiteSpace(issuer))
                {
                    // internal AD
                    if (_azureADValidationConfig.ValidIssuers.Contains(issuer))
                    {
                        // Control Azure AD internal application id
                        appId = context?.User?.FindFirst(ApiConstants.AzureADInternalApplicationIdClaimİdentifier)?.Value;
                        if (!string.IsNullOrWhiteSpace(appId))
                            applicationIds.Add(appId);
                    }

                    // We check whther the incoming requests application ids match with the required ids
                    appIdsIntersect = requirement.AppIds.Intersect(applicationIds).ToList();

                    if (!appIdsIntersect.Any())
                        return; // this will return unauthorized if the requests does not included allowed application ids
                }
                else
                    return;
            }

            context.Succeed(requirement);
        }

    }
}
