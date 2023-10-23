using HowToSecureAPI.Demo.Configs.OnlyAzureADB2CValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace HowToSecureAPI.Demo.Extensions
{
    /// <summary>
    ///  Azure AD B2C
    ///  Add Authentication For Token Validation
    /// </summary>
    public static class AzureADB2CAddAuthenticationForTokenValidation
    {
        private const string AzureB2CAuthorizationServer = "AzureB2CAuthorizationServer";

        /// <summary>
        ///  This extension class is created for validate Azure AD B2C token
        /// </summary>
        /// <param name="services"></param>
        /// <param name="azureB2CValidationConfig"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationForAzureADB2C(this IServiceCollection services, AzureB2CValidationConfig azureB2CValidationConfig)
        {
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(azureB2CValidationConfig.MetadataAddress, new OpenIdConnectConfigurationRetriever());

            var openidconfig = configManager.GetConfigurationAsync().Result;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                // Azure AD B2C TOKEN VAlAIDATION
                .AddJwtBearer(AzureB2CAuthorizationServer, options =>
                {
                    options.Authority = azureB2CValidationConfig.Authority;
                    options.MetadataAddress = azureB2CValidationConfig.MetadataAddress;

                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudiences = azureB2CValidationConfig.ValidAudiences,
                        ValidIssuers = azureB2CValidationConfig.ValidIssuers,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = openidconfig.SigningKeys,
                    };
                });

            return services;
        }
    }
}
