using AzureFunctionAuthorization.Domain.ViewModel;
using AzureFunctionAuthorization.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Threading.Tasks;

namespace AzureFunctionAuthorization.Binding {
    public class JWTAuthorizationBinding : IBinding {

        private readonly IJWTHandlerService _jWTHandlerService;
        public JWTAuthorizationBinding(IJWTHandlerService jWTHandlerService) {
            _jWTHandlerService = jWTHandlerService;
        }
        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(BindingContext context) {
            var data = context.BindingData["req"] as HttpRequest;
            return Task.FromResult<IValueProvider>(new JWTTokenProvider(data,_jWTHandlerService));
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) {
            return null;
        }

        public ParameterDescriptor ToParameterDescriptor() {
            return null;
        }
    }

    public class JWTTokenProvider:IValueProvider {
        private HttpRequest _httprequest;
        private readonly IJWTHandlerService _jWTHandlerService;
        
        public JWTTokenProvider(HttpRequest httpRequest, IJWTHandlerService jWTHandlerService) {
            _httprequest = httpRequest;
            _jWTHandlerService = jWTHandlerService;
        }

        public async Task<object> GetValueAsync() {
            var request =  await _httprequest.ReadAsStringAsync() as object;
            string token = _httprequest.Headers["Authorization"];
            JWTClaim claim = null;
            if (string.IsNullOrWhiteSpace(token)) { return claim; }
            try
            {
                claim = _jWTHandlerService.ParseClaims(token);
            }
            catch (Exception e)
            {
                return claim;
            }
            return claim;
        }

        public string ToInvokeString() => string.Empty;
        public Type Type => typeof(object);
    }
}
