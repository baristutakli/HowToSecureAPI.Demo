using HowToSecureAPI.Demo.Configs.MultipleValidation.AzureADAndB2CWithOkta;
using HowToSecureAPI.Demo.Configs.MultipleValidationIncludingOkta.OnlyAzureADAndB2C;
using HowToSecureAPI.Demo.Constants;
using HowToSecureAPI.Demo.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HowToSecureAPI.Demo.Handlers.B2CAndAzureADInternal
{
    /// <summary>
    /// Check whether an application is allowed to make request to this api or not
    /// </summary>
    public class AzureADAndB2CAllowedApplicationsAuthorizationHandler : AuthorizationHandler<AllowedApplicationsRequirement>
    {
        private readonly AzureADAndB2CValidationConfig _azureADAndB2CValidationConfig;
        /// <summary>
        /// CTOR
        /// </summary>
        public AzureADAndB2CAllowedApplicationsAuthorizationHandler(AzureADAndB2CValidationConfig azureADAndB2CValidationConfig)
        {
            // Inject the followings if you want to trace the flow
            // inject logger 
            // inject metric agent
            _azureADAndB2CValidationConfig = azureADAndB2CValidationConfig;
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
                string? appId = context?.User?.FindFirst(ApiConstants.AzureB2CApplicationIdClaimİdentifier)?.Value;
                if (!string.IsNullOrWhiteSpace(issuer))
                {
                    // B2C
                    if (_azureADAndB2CValidationConfig.AzureADB2C.ValidIssuers.Contains(issuer))
                    {
                        // Control Azure B2C application id 
                        if (!string.IsNullOrWhiteSpace(appId))
                            applicationIds.Add(appId);
                    }
                    // internal AD
                    else if (_azureADAndB2CValidationConfig.AzureAD.ValidIssuers.Contains(issuer))
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
