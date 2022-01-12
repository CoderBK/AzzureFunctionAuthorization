using Microsoft.Azure.WebJobs.Description;
using System;

namespace AzureFunctionAuthorization.Attributes {
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class JWTAuthorizationAttribute : Attribute {
    }
}
