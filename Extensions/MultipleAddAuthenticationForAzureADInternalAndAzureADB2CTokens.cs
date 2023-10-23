using HowToSecureAPI.Demo.Configs.MultipleValidationIncludingOkta.OnlyAzureADAndB2C;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Extensions
{
    /// <summary>
    /// Azure Ad and Azure AD B2C
    /// Add Authentication For Azure AD Internal And Azure AD B2C Tokens
    /// </summary>
    public static class MultipleAddAuthenticationForAzureADInternalAndAzureADB2CTokens
    {
        private const string InternalAzureAuthorizationServer = "InternalAzureAuthorizationServer";
        private const string AzureB2CAuthorizationServer = "AzureB2CAuthorizationServer";

        /// <summary>
        /// This extension class is created for validate Azure AD and Azure AD B2C tokens
        /// </summary>
        /// <param name="services"></param>
        /// <param name="multipleValidationConfig"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationForAzureADInternalAndAzureADB2C(this IServiceCollection services, AzureADAndB2CValidationConfig azureADAndB2CValidationConfig)
        {
            var InternalADconfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(azureADAndB2CValidationConfig.AzureAD.MetadataAddress, new OpenIdConnectConfigurationRetriever());
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(azureADAndB2CValidationConfig.AzureADB2C.MetadataAddress, new OpenIdConnectConfigurationRetriever());

            var InternalADopenidconfig = InternalADconfigManager.GetConfigurationAsync().Result;
            var openidconfig = configManager.GetConfigurationAsync().Result;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                // Azure AD INTERNAL TOKEN VAlAIDATION
                .AddJwtBearer(InternalAzureAuthorizationServer, options =>
                {
                    options.MetadataAddress = azureADAndB2CValidationConfig.AzureAD.MetadataAddress;

                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudiences = azureADAndB2CValidationConfig.AzureAD.ValidAudiences,

                        ValidateIssuer = true,
                        ValidIssuers = azureADAndB2CValidationConfig.AzureAD.ValidIssuers,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = InternalADopenidconfig.SigningKeys,

                        ValidateLifetime = true,
                        ValidTypes = new List<string>() { "JWT" }

                    };
                })
                // Azure AD B2C TOKEN VAlAIDATION
                .AddJwtBearer(AzureB2CAuthorizationServer, options =>
                {
                    options.Authority = azureADAndB2CValidationConfig.AzureADB2C.Authority;
                    options.MetadataAddress = azureADAndB2CValidationConfig.AzureADB2C.MetadataAddress;

                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudiences = azureADAndB2CValidationConfig.AzureADB2C.ValidAudiences,
                        ValidIssuers = azureADAndB2CValidationConfig.AzureADB2C.ValidIssuers,
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
