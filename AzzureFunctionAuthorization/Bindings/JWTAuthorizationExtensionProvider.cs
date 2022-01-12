using AzureFunctionAuthorization.Attributes;
using AzureFunctionAuthorization.IService;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace AzureFunctionAuthorization.Binding {

    [Extension(name: "JWTAuthorizationBinding")]
    public class JWTAuthorizationExtensionProvider : IExtensionConfigProvider{

        private readonly IJWTHandlerService _jWTHandlerService;
        public JWTAuthorizationExtensionProvider(IJWTHandlerService jWTHandlerService) {
            _jWTHandlerService = jWTHandlerService;
        }
        

        public void Initialize(ExtensionConfigContext context) {
            var binding = context.AddBindingRule<JWTAuthorizationAttribute>()
                .Bind(new JWTAuthorizationBindingProvider(_jWTHandlerService));
        }
    }
}
