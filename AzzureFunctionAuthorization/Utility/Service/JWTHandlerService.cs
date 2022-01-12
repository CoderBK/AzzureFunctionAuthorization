using AzureFunctionAuthorization.Domain.ViewModel;
using AzureFunctionAuthorization.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AzureFunctionAuthorization.Service {
    public class JWTHandlerService : IJWTHandlerService {
        private readonly JWTConfiguration _jWTConfiguration;
        private readonly SigningCredentials _accessTokenCred;

        public JWTHandlerService(
            JWTConfiguration jWTConfiguration
        ) {
            this._jWTConfiguration = jWTConfiguration;
            this._accessTokenCred = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jWTConfiguration.AccessTokenSecret)),
                SecurityAlgorithms.HmacSha256
            );
        }

        /// <summary>
        /// Creating JWT
        /// </summary>
        public string GenerateAccessToken(JWTClaim claim) {
            // Create claims
            var claims = this.GetClaims(claim);

            // Prepare token object
            var token = new JwtSecurityToken(
                this._jWTConfiguration.Issuer,
                this._jWTConfiguration.Audience,
                claims,
                expires: DateTime.UtcNow.AddMilliseconds(this._jWTConfiguration.AccessTokenExpiryTimeMiliseconds),
                signingCredentials: this._accessTokenCred
            );

            // Write token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Creates refresh token
        /// </summary>
        public string GenerateRefreshToken(out DateTime expiry) {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32]; //256 bit
                cryptoProvider.GetBytes(secretKeyByteArray);
                expiry = DateTime.UtcNow.AddMilliseconds(this._jWTConfiguration.RefreshTokenExpiryTimeMiliseconds);
                return Convert.ToBase64String(secretKeyByteArray);
            }
        }

        /// <summary>
        /// Gets principal payload from token
        /// </summary>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token) {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jWTConfiguration.AccessTokenSecret)),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                    || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException();
                }

                return principal;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Parse principal payload to create JWTClaim
        /// </summary>
        public JWTClaim ParseClaims(ClaimsPrincipal principal) {
            try
            {
                JWTClaim claim = new JWTClaim {
                    AccountId = int.Parse(principal.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sid).FirstOrDefault()?.Value)
                };

                return claim;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Parse JWT token payload to create JWTClaim
        /// </summary>
        public JWTClaim ParseClaims(string token) {
            // Parse token from raw header string
            token = this.ParseTokenFromRawString(token);

            // Check if the token is correct
            if (!this.CanReadToken(token))
            {
                throw new Exception();
            }

            // Parse claims from token
            return this.ParseClaims(this.GetPrincipalFromExpiredToken(token));
        }

        /// <summary>
        /// Validates if token is correct
        /// </summary>
        public bool CanReadToken(string token) {
            return new JwtSecurityTokenHandler().CanReadToken(token);
        }

        /// <summary>
        /// Replaces "Bearer " from raw token
        /// </summary>
        public string ParseTokenFromRawString(string token) {
            return Regex.Replace(token, JwtBearerDefaults.AuthenticationScheme + " ", "", RegexOptions.IgnoreCase);
        }

        private Claim[] GetClaims(JWTClaim claim) {
            return new[] {
                new Claim(JwtRegisteredClaimNames.Sid, claim.AccountId.ToString()),
            };
        }
    }
}
