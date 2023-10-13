using System.Collections.Generic;
using HowToSecureAPI.Demo.Configs.MultipleValidation;

namespace HowToSecureAPI.Demo.Configs.MultipleValidation.AzureADAndB2CWithOkta
{
    /// <summary>
    /// OKTA, AZURE AD AND AZURE AD B2C 
    /// </summary>
    public class MultipleValidationConfig
    {
        public AzureADB2C AzureADB2C { get; set; }
        public AzureAD AzureAD { get; set; }
        public Okta Okta { get; set; }
        public List<string> AllowedAppIds { get; set; }
    }

}

