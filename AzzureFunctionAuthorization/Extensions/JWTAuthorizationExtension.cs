using AzureFunctionAuthorization.Binding;
using Microsoft.Azure.WebJobs;

namespace AzureFunctionAuthorization.Extensions {
    public static class JWTAuthorizationExtension {
        public static IWebJobsBuilder AddJWTAuthorizationBinding(this IWebJobsBuilder jobsBuilder) {
            jobsBuilder.AddExtension<JWTAuthorizationExtensionProvider>();
            return jobsBuilder;
        }
    }
}
