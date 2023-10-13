using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Configs.OnlyAzureADInternalValidation
{
    public class AzureADValidationConfig
    {
        public List<string> AllowedAppIds { get; set; }
        public string MetadataAddress { get; set; }
        public List<string> ValidAudiences { get; set; }
        public List<string> ValidIssuers { get; set; }
    }

}


