using AzureFunctionAuthorization.Binding;
using AzureFunctionAuthorization.Domain.ViewModel;
using AzureFunctionAuthorization.IService;
using AzureFunctionAuthorization.Service;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(AzureFunctionAuthorization.Startup))]
namespace AzureFunctionAuthorization {
    
    public class Startup : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {
            var configuration = builder.GetContext().Configuration;

            var services = builder.Services;

            // Register JWTHandlerSwervice, used to create parse and validate token
            services.AddSingleton<IJWTHandlerService, JWTHandlerService>(x => {
                return new JWTHandlerService(
                    new JWTConfiguration {
                        AccessTokenSecret = configuration["Token:AccessTokenSecret"],
                        AccessTokenExpiryTimeMiliseconds = Convert.ToInt64(configuration["Token:AccessTokenExpiryTime"]),
                        RefreshTokenExpiryTimeMiliseconds = Convert.ToInt64(configuration["Token:RefreshTokenExpiryTime"]),
                        Audience = configuration["Token:Audience"],
                        Issuer = configuration["Token:Issuer"],
                    }
                );
            });

            // Register JWTAuthorizationExtesionProvider
            var webJobs = services.AddWebJobs(x => { return; });
            webJobs.AddExtension<JWTAuthorizationExtensionProvider>();
        }
    }
}
