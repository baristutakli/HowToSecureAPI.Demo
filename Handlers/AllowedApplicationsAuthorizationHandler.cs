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
    /// <summary>
    /// Check whether an application is allowed to make request to this api or not
    /// </summary>
    public class AllowedApplicationsAuthorizationHandler : AuthorizationHandler<AllowedApplicationsRequirement>
    {
        private readonly MultipleValidationConfig _multipleValidationConfig;
        /// <summary>
        /// CTOR
        /// </summary>
        public AllowedApplicationsAuthorizationHandler(MultipleValidationConfig multipleValidationConfig)
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
                    if (_multipleValidationConfig.AzureADB2C.ValidIssuers.Contains(issuer))
                    {
                        // Control Azure B2C application id 
                        if (!string.IsNullOrWhiteSpace(appId))
                            applicationIds.Add(appId);
                    }
                    // internal AD
                    else if (_multipleValidationConfig.AzureAD.ValidIssuers.Contains(issuer))
                    {
                        // Control Azure AD internal application id
                        appId = context?.User?.FindFirst(ApiConstants.AzureADInternalApplicationIdClaimİdentifier)?.Value;
                        if (!string.IsNullOrWhiteSpace(appId))
                            applicationIds.Add(appId);
                    }
                    // Okta
                    else if (_multipleValidationConfig.Okta.ValidIssuers.Contains(issuer))
                    {
                        // Control Okta id 
                        appId = context?.User?.FindFirst(ApiConstants.OktaApplicationSubClaimİdentifier)?.Value;
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
