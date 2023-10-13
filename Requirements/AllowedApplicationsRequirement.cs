using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Requirements
{
    public class AllowedApplicationsRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// List of application ids required for authorization
        /// Add only the needed application ids
        /// </summary>
        public List<string> AppIds { get; }

        /// <summary>
        /// application ids requirement
        /// Consumer must have at least one of the required application ids to get authorized
        /// </summary>
        /// <param name="appIds">List of required application ids for authorization</param>
        public AllowedApplicationsRequirement(List<string> appIds)
        {
            AppIds = appIds ?? throw new ArgumentNullException(nameof(appIds));
        }
    }
}
