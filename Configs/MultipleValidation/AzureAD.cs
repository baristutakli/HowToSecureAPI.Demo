using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Configs.MultipleValidation
{
    public class AzureAD
    {
        public string MetadataAddress { get; set; }
        public List<string> ValidAudiences { get; set; }
        public List<string> ValidIssuers { get; set; }
    }

}

