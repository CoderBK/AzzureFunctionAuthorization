using AzureFunctionAuthorization.IService;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System.Threading.Tasks;

namespace AzureFunctionAuthorization.Binding {

    public class JWTAuthorizationBindingProvider : IBindingProvider {
        private readonly IJWTHandlerService _jWTHandlerService;

        public JWTAuthorizationBindingProvider(IJWTHandlerService jWTHandlerService) {
            _jWTHandlerService = jWTHandlerService;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context) {
          return Task.FromResult<IBinding>(new JWTAuthorizationBinding(_jWTHandlerService));
            
        }
    }
}
