using AzureFunctionAuthorization.Domain.ViewModel;
using System;
using System.Security.Claims;

namespace AzureFunctionAuthorization.IService {
    public interface IJWTHandlerService {
        string GenerateAccessToken(JWTClaim claim);

        string GenerateRefreshToken(out DateTime expiry);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        JWTClaim ParseClaims(ClaimsPrincipal principal);

        JWTClaim ParseClaims(string token);

        bool CanReadToken(string token);

        string ParseTokenFromRawString(string token);
    }
}
