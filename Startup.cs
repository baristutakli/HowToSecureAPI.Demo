using HowToSecureAPI.Demo.Configs.OnlyAzureADB2CValidation;
using HowToSecureAPI.Demo.Configs.Swagger;
using HowToSecureAPI.Demo.Constants;
using HowToSecureAPI.Demo.Extensions;
using HowToSecureAPI.Demo.Extensions.Swagger;
using HowToSecureAPI.Demo.Handlers.B2C;
using HowToSecureAPI.Demo.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace HowToSecureAPI.Demo
{
    public class Startup
    {

        private const string AzureB2CAuthorizationServer = "AzureB2CAuthorizationServer";
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

            // Uncomment the code below for Azure AD B2C Only
            // Only Azure AD B2C apps allowed
            services.AddSingleton<IAuthorizationHandler, B2CAppScopeAuthorizationHandler>(); // supportAzure AD B2C
            services.AddSingleton<IAuthorizationHandler, AzureB2CAllowedApplicationsAuthorizationHandler>(); // Azure AD B2C Applications AuthorizationHandler

            #endregion

            // Add Authorization for endpoints 
            services.AddAuthorization(options =>
            {

                // Uncomment the code below for Azure AD B2C Only
                // Only Azure AD B2C apps allowed
                // Don't forget to change required scopes for your own scopes
                options.AddPolicy(ApiConstants.UserReadPolicyName, policyBuilder =>
                    policyBuilder
                        .AddAuthenticationSchemes( AzureB2CAuthorizationServer)
                        .AddRequirements(new AllowedApplicationsRequirement(azureB2CValidationConfig.AllowedAppIds))
                .AddRequirements(new B2CAppScopeRequirement(new List<string> { ApiConstants.UsersReadScope, ApiConstants.UsersManageScope })).Build());

            });

            // Uncomment the code below for Azure AD B2C Only
            #region Token Validation for Azure AD B2C
            services.AddAuthenticationForAzureADB2C(azureB2CValidationConfig);
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
