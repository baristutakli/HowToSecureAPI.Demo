using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Configs.OnlyAzureADB2CValidation
{
    public class AzureB2CValidationConfig
    {
        public string Authority { get; set; }
        public List<string> AllowedAppIds { get; set; }
        public string MetadataAddress { get; set; }
        public List<string> ValidAudiences { get; set; }
        public List<string> ValidIssuers { get; set; }
    }
}



