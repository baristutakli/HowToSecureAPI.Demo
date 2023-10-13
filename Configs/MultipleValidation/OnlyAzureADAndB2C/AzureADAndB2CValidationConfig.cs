using System.Collections.Generic;
using HowToSecureAPI.Demo.Configs.MultipleValidation;

namespace HowToSecureAPI.Demo.Configs.MultipleValidationIncludingOkta.OnlyAzureADAndB2C
{
    /// <summary>
    /// AZURE AD AND AZURE AD B2C 
    /// </summary>
    public class AzureADAndB2CValidationConfig
    {
        public AzureADB2C AzureADB2C { get; set; }
        public AzureAD AzureAD { get; set; }
        public List<string> AllowedAppIds { get; set; }
    }

}

