using HowToSecureAPI.Demo.Configs.MultipleValidation.AzureADAndB2CWithOkta;
using HowToSecureAPI.Demo.Configs.MultipleValidationIncludingOkta.OnlyAzureADAndB2C;
using HowToSecureAPI.Demo.Configs.OnlyAzureADB2CValidation;
using HowToSecureAPI.Demo.Configs.OnlyAzureADInternalValidation;
using HowToSecureAPI.Demo.Configs.Swagger;
using HowToSecureAPI.Demo.Constants;
using HowToSecureAPI.Demo.Extensions;
using HowToSecureAPI.Demo.Extensions.Swagger;
using HowToSecureAPI.Demo.Handlers;
using HowToSecureAPI.Demo.Handlers.B2C;
using HowToSecureAPI.Demo.Handlers.B2CAndAzureADInternal;
using HowToSecureAPI.Demo.Handlers.InternalAD;
using HowToSecureAPI.Demo.Requirements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace HowToSecureAPI.Demo
{
    public class Startup
    {

        private const string InternalAzureAuthorizationServer = "InternalAzureAuthorizationServer";
        private const string AzureB2CAuthorizationServer = "AzureB2CAuthorizationServer";
        private const string OktaAuthorizationServer = "OktaAuthorizationServer";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region Reading configs from appsettings and registering configs
            // Azure AD B2C Swagger 
            var azureADB2CSwaggerConfig = Configuration.GetSection("AzureADB2CSwaggerConfig").Get<AzureADB2CSwaggerConfig>();
            services.AddSingleton(azureADB2CSwaggerConfig);

            // Azure AD B2C Validation Config
            // If you have application registered in Azure AD B2C
            var azureB2CValidationConfig = Configuration.GetSection("AzureB2CValidationConfig").Get<AzureB2CValidationConfig>();
            services.AddSingleton(azureB2CValidationConfig);

            // Azure AD Validation Config
            // If you have application registered in  Azure AD Internal
            var azureADvalidationconfig = Configuration.GetSection("AzureADvalidationconfig").Get<AzureADValidationConfig>();
            services.AddSingleton(azureADvalidationconfig);

            // Azure AD Internal and Azure AD B2C validation Config
            // If you have application registered in Azure AD B2C and Azure AD Internal
            var azureADAndB2CValidationConfig = Configuration.GetSection("AzureADAndB2CValidationConfig").Get<AzureADAndB2CValidationConfig>();
            services.AddSingleton(azureADAndB2CValidationConfig);


            // !!! Only use this if you have too coupled apps till the deadline
            // Azure AD, Azure AD B2C and Okta token validation
            // If you have application registered in Azure AD B2C and Azure AD Internal and Okta
            var multipleValidationConfig = Configuration.GetSection("MultipleValidationConfig").Get<MultipleValidationConfig>();
            services.AddSingleton(multipleValidationConfig);
            #endregion


            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
            });


            #region Register Authorization Handler for different purposes

            // Uncomment the code below for Azure AD Only
            // Only Azure AD INTERNAL apps allowed
            services.AddSingleton<IAuthorizationHandler, InternalAppScopeAuthorizationHandler>(); // Internal scope Requirement
            services.AddSingleton<IAuthorizationHandler, InternalADAllowedApplicationsAuthorizationHandler>(); // Internal AD Applications AuthorizationHandler


            // Uncomment the code below for Azure AD B2C Only
            // Only Azure AD B2C apps allowed
            services.AddSingleton<IAuthorizationHandler, B2CAppScopeAuthorizationHandler>(); // supportAzure AD B2C
            services.AddSingleton<IAuthorizationHandler, AzureB2CAllowedApplicationsAuthorizationHandler>(); // Azure AD B2C Applications AuthorizationHandler


            // Uncomment the code below for Azure AD and Azure AD B2C Token Validation
            // support Azure AD , Azure AD B2C
            services.AddSingleton<IAuthorizationHandler, B2CAndAzureADInternalTokenScopeAuthorizationHandler>(); // support Azure AD , Azure AD B2C
            services.AddSingleton<IAuthorizationHandler, AzureADAndB2CAllowedApplicationsAuthorizationHandler>(); // Azure AD B2C Applications AuthorizationHandler


            // Uncomment the code below for Multiple Token Validation including Okta token
            // support Azure AD , Azure AD B2C and Okta
            services.AddSingleton<IAuthorizationHandler, MultipleIdentityProviderTokenScopeAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AllowedApplicationsAuthorizationHandler>(); // support Azure AD , Azure AD B2C and Okta

            #endregion

            services.AddAuthorization(options =>
            {
                // Uncomment the code below for Azure AD Only
                // Only Azure AD INTERNAL apps allowed
                // Don't forget to change required scopes for your own scopes
                options.AddPolicy(ApiConstants.InternalUserReadPolicyName, policyBuilder =>
                    policyBuilder
                        .AddAuthenticationSchemes(InternalAzureAuthorizationServer)
                        .AddRequirements(new AllowedApplicationsRequirement(azureADvalidationconfig.AllowedAppIds))
                .AddRequirements(new InternalAppScopeRequirement(new List<string> { ApiConstants.InternalUserReadScope })).Build());

                // Uncomment the code below for Azure AD B2C Only
                // Only Azure AD B2C apps allowed
                // Don't forget to change required scopes for your own scopes
                //options.AddPolicy(ApiConstants.UserReadPolicyName, policyBuilder =>
                //    policyBuilder
                //        .AddAuthenticationSchemes(InternalAzureAuthorizationServer, AzureB2CAuthorizationServer)
                //        .AddRequirements(new AllowedApplicationsRequirement(azureB2CValidationConfig.AllowedAppIds))
                //.AddRequirements(new B2CAppScopeRequirement(new List<string> { ApiConstants.UsersReadScope, ApiConstants.UsersManageScope})).Build());


                // Uncomment the code below for Azure AD and Azure AD B2C Token Validation
                //Internal AD And B2C User Read Policy Name
                // Azure AD B2C and Azure AD Token validation
                // Don't forget to change required scopes for your own scopes
                //options.AddPolicy(ApiConstants.InternalAndB2CUserReadPolicyName, policyBuilder =>
                //    policyBuilder
                //        .AddAuthenticationSchemes(InternalAzureAuthorizationServer, AzureB2CAuthorizationServer)
                //        .AddRequirements(new AllowedApplicationsRequirement(azureADAndB2CValidationConfig.AllowedAppIds))
                //.AddRequirements(new B2CAndAzureADInternalScopeRequirement(new List<string> { ApiConstants.UsersReadScope, ApiConstants.UsersManageScope, ApiConstants.InternalUserReadScope })).Build());


                // Uncomment the code below for Multiple Token Validation including Okta token
                //Azure AD B2C, Azure AD and OKTA Token validation
                // Don't forget to change required scopes for your own scopes
                //options.AddPolicy(ApiConstants.MultipleValidationUserReadPolicyName, policyBuilder =>
                //    policyBuilder
                //        .AddAuthenticationSchemes(InternalAzureAuthorizationServer, AzureB2CAuthorizationServer, OktaAuthorizationServer)
                //        .AddRequirements(new AllowedApplicationsRequirement(multipleValidationConfig.AllowedAppIds))
                //.AddRequirements(new MultipleIdentityProviderScopeRequirement(new List<string> { ApiConstants.UsersReadScope, ApiConstants.UsersManageScope, ApiConstants.InternalUserReadScope,  })).Build());


            });

            // Uncomment the code below for Azure AD B2C Only
            #region Token Validation for Azure AD 
            services.AddAuthenticationForAzureADB2C(azureB2CValidationConfig);
            #endregion

            // Uncomment the code below for Azure AD Only
            #region Token Validation for Azure AD 
            services.AddAuthenticationForAzureADInternal(azureADvalidationconfig);
            #endregion

            // Uncomment the code below for Azure AD and Azure AD B2C Token Validation
            #region Multiple Token Validation for Azure AD and Azure AD B2C
            //services.AddAuthenticationForAzureADInternalAndAzureADB2C(azureADAndB2CValidationConfig);
            #endregion


            // Uncomment the code below for Multiple Token Validation including Okta token
            #region Multiple Token Validation for Azure AD, Azure AD B2C and Okta
            //services.AddMultipleAuthentication(multipleValidationConfig);
            #endregion

            services.AddVersionedApiExplorer();
            services.SetupSwaggerDocumentation(azureADB2CSwaggerConfig);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, AzureADB2CSwaggerConfig azureADB2CSwaggerConfig)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerDocumentation(provider, azureADB2CSwaggerConfig);
        }
    }
}
