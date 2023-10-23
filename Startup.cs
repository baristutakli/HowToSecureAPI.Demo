using HowToSecureAPI.Demo.Configs.OnlyAzureADInternalValidation;
using HowToSecureAPI.Demo.Constants;
using HowToSecureAPI.Demo.Extensions;
using HowToSecureAPI.Demo.Handlers.InternalAD;
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

        private const string InternalAzureAuthorizationServer = "InternalAzureAuthorizationServer";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //////////////////////////////////////////////////////////////////////////////////////
            /*
             * This branch convers only Azure AD token authentication 
             * Does not have swagger
             * The prupose is to remove unnecessary details if you have only Azure AD application
             */
            //////////////////////////////////////////////////////////////////////////////////////


            #region Reading configs from appsettings and registering configs

            // Azure AD Validation Config
            // If you have application registered in  Azure AD Internal
            var azureADvalidationconfig = Configuration.GetSection("AzureADvalidationconfig").Get<AzureADValidationConfig>();
            services.AddSingleton(azureADvalidationconfig);

            #endregion

            #region Register Authorization Handler for different purposes

            // Uncomment the code below for Azure AD Only
            // Only Azure AD INTERNAL apps allowed
            services.AddSingleton<IAuthorizationHandler, InternalAppScopeAuthorizationHandler>(); // Internal scope Requirement
            services.AddSingleton<IAuthorizationHandler, InternalADAllowedApplicationsAuthorizationHandler>(); // Internal AD Applications AuthorizationHandler

            #endregion

            // Add Authorization for endpoints 
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

            });


            // Uncomment the code below for Azure AD Only
            #region Token Validation for Azure AD 
            services.AddAuthenticationForAzureADInternal(azureADvalidationconfig);
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
