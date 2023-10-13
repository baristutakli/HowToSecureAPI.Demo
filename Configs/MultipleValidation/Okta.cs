using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Configs.MultipleValidation
{
    public class Okta
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public List<string> ValidAudiences { get; set; }
        public List<string> ValidIssuers { get; set; }
    }

}

