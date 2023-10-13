using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Requirements
{
    public class MultipleIdentityProviderScopeRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// List of scopes required for authorization
        /// Add only the needed scopes 
        /// </summary>
        public List<string> Scopes { get; }

        /// <summary>
        /// Scope requirement.
        /// Consumer must have at least one of the required scopes to get authorized
        /// </summary>
        /// <param name="scopes">List of scopes required for authorization</param>
        public MultipleIdentityProviderScopeRequirement(List<string> scopes)
        {
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }
    }
}
