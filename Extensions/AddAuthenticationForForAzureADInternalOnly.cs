using HowToSecureAPI.Demo.Configs.MultipleValidation.AzureADAndB2CWithOkta;
using HowToSecureAPI.Demo.Configs.OnlyAzureADB2CValidation;
using HowToSecureAPI.Demo.Configs.OnlyAzureADInternalValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using HowToSecureAPI.Demo.Configs.MultipleValidationIncludingOkta.OnlyAzureADAndB2C;

namespace HowToSecureAPI.Demo.Extensions
{
    /// <summary>
    /// Multiple Add Authentication For Token Validation Including Okta
    /// </summary>
    public static class AddAuthenticationForForAzureADInternalOnly
    {
        private const string InternalAzureAuthorizationServer = "InternalAzureAuthorizationServer";

        /// <summary>
        /// This extension class is created for validate Azure AD
        /// </summary>
        /// <param name="services"></param>
        /// <param name="azureADvalidationconfig"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationForAzureADInternal(this IServiceCollection services, AzureADValidationConfig azureADvalidationconfig)
        {
            var InternalADconfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(azureADvalidationconfig.MetadataAddress, new OpenIdConnectConfigurationRetriever());

            var InternalADopenidconfig = InternalADconfigManager.GetConfigurationAsync().Result;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                // Azure AD INTERNAL TOKEN VAlAIDATION
                .AddJwtBearer(InternalAzureAuthorizationServer, options =>
                {
                    options.MetadataAddress = azureADvalidationconfig.MetadataAddress;

                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudiences = azureADvalidationconfig.ValidAudiences,

                        ValidateIssuer = true,
                        ValidIssuers = azureADvalidationconfig.ValidIssuers,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = InternalADopenidconfig.SigningKeys,

                        ValidateLifetime = true,
                        ValidTypes = new List<string>() { "JWT" }

                    };
                });
                

            return services;
        }
    }
}
