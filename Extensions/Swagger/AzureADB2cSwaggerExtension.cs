﻿using HowToSecureAPI.Demo.Configs.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HowToSecureAPI.Demo.Extensions.Swagger
{
    /// <summary>
    /// Extension for swagger documentation
    /// </summary>
    public static class AzureADB2cSwaggerExtension
    {
        public static IServiceCollection SetupSwaggerDocumentation(this IServiceCollection services,
          AzureADB2CSwaggerConfig swaggerConfig)
        {



            services.AddSwaggerGen(options =>
            {
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));

                //Locate the XML file being generated by ASP.NET...
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //... and tell Swagger to use those XML comments.
                options.IncludeXmlComments(xmlPath);


                var scope = new Dictionary<string, string>();

                foreach (var rightName in swaggerConfig.OAuthScopesAndDescriptions)
                {
                    scope.Add(rightName.Value, $"{rightName.Key}");
                }

                var flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(swaggerConfig.AuthorizationUrl),
                        TokenUrl = new Uri(swaggerConfig.TokenUrl),
                        Scopes = scope
                    }
                };


                //First we define the security scheme
                var securityDefinitionName = "oauth2";
                options.AddSecurityDefinition(securityDefinitionName, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = flows,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                // to make sure that the bearer token is send in authorization
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = securityDefinitionName, //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });

            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //});

            return services;
        }
        /// <summary>
        /// Add use of swagger to IApplicationBuilder
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="provider">IApiVersionDescriptionProvider</param>
        /// <param name="swaggerConfig">SwaggerConfig</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider, AzureADB2CSwaggerConfig graphSwaggerConfig)
        {
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = graphSwaggerConfig.ApiServerUris.Select(uri => new OpenApiServer { Url = uri }).ToList();
                });
            });

            app.UseSwaggerUI(options =>
            {

                options.DisplayOperationId();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
                options.OAuthClientId(graphSwaggerConfig.OAuthClientId);

                options.OAuthAppName(graphSwaggerConfig.OAuthClientName);

                options.OAuthUsePkce();

                // look at it later ToDo
                options.OAuth2RedirectUrl(graphSwaggerConfig.RedirectUri);

            });

            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});
            return app;
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Version = "v1",
                Title = "How To Secure API.Demo API Swagger Specification",
                Description = "This demo API is created to show you how to secure your dotnet api using Azure AD B2C",
                TermsOfService = new Uri("https://demoapi.com/terms"),// random url
                Contact = new OpenApiContact
                {
                    Name = "Baris Tutakli",
                    Email = "https://www.linkedin.com/in/baristutakli/",
                    // Url = new Uri("")
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
