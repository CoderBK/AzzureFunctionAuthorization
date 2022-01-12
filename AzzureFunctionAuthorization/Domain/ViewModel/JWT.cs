using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionAuthorization.Domain.ViewModel {
    public class JWTClaim {
        public int AccountId { get; set; }
    }

    public class JWTConfiguration {
        public string AccessTokenSecret { get; set; }
        public long AccessTokenExpiryTimeMiliseconds { get; set; }
        public long RefreshTokenExpiryTimeMiliseconds { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
