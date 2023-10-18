using HowToSecureAPI.Demo.Configs.MultipleValidation.AzureADAndB2CWithOkta;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace HowToSecureAPI.Demo.Extensions
{
    /// <summary>
    /// Multiple Add Authentication For Token Validation Including Okta
    /// </summary>
    public static class MultipleAddAuthenticationForTokenValidationIncludingOkta
    {
        private const string InternalAzureAuthorizationServer = "InternalAzureAuthorizationServer";
        private const string AzureB2CAuthorizationServer = "AzureB2CAuthorizationServer";
        private const string OktaAuthorizationServer = "OktaAuthorizationServer";

        /// <summary>
        /// Azure AD, Azure AD B2C and Okta token validation
        /// </summary>
        /// <param name="services"></param>
        /// <param name="multipleValidationConfig"></param>
        /// <returns></returns>
        public static IServiceCollection AddMultipleAuthentication(this IServiceCollection services, MultipleValidationConfig multipleValidationConfig)
        {
            var InternalADconfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(multipleValidationConfig.AzureAD.MetadataAddress, new OpenIdConnectConfigurationRetriever());
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(multipleValidationConfig.AzureADB2C.MetadataAddress, new OpenIdConnectConfigurationRetriever());

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
                    options.MetadataAddress = multipleValidationConfig.AzureAD.MetadataAddress;

                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudiences = multipleValidationConfig.AzureAD.ValidAudiences,

                        ValidateIssuer = true,
                        ValidIssuers = multipleValidationConfig.AzureAD.ValidIssuers,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = InternalADopenidconfig.SigningKeys,

                        ValidateLifetime = true,
                        ValidTypes = new List<string>() { "JWT" }

                    };
                })
                // Azure AD B2C TOKEN VAlAIDATION
                .AddJwtBearer(AzureB2CAuthorizationServer, options =>
                {
                    options.Authority = multipleValidationConfig.AzureADB2C.Authority;
                    //options.Audience = "13cfb580-5366-4b81-b90b-92a1dca8879b";
                    options.MetadataAddress = multipleValidationConfig.AzureADB2C.MetadataAddress;

                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudiences = multipleValidationConfig.AzureADB2C.ValidAudiences,
                        ValidIssuers = multipleValidationConfig.AzureADB2C.ValidIssuers,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = openidconfig.SigningKeys,
                    };
                }).AddJwtBearer(OktaAuthorizationServer, options =>
                {
                    options.Authority = multipleValidationConfig.Okta.Authority; 
                    options.Audience = multipleValidationConfig.Okta.Audience;
                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudiences = multipleValidationConfig.Okta.ValidAudiences,
                        ValidateLifetime = true
                        // feel free to check signinkey and other parameters
                    };

                });

            return services;
        }
    }
}
