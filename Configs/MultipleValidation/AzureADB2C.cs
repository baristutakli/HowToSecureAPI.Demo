using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Configs.MultipleValidation
{
    public class AzureADB2C
    {
        public string Authority { get; set; }
        public string MetadataAddress { get; set; }
        public List<string> ValidAudiences { get; set; }
        public List<string> ValidIssuers { get; set; }
    }

}

