using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Configs.Swagger
{
    public class AzureADB2CSwaggerConfig
    {
        public List<string> ApiServerUris { get; set; }
        public string OAuthClientId { get; set; }
        public string OAuthClientName { get; set; }
        public string RedirectUri { get; set; }

        public string AuthorizationUrl { get; set; }

        public string TokenUrl { get; set; }

        public Dictionary<string, string> OAuthScopesAndDescriptions { get; set; }

    }
}



