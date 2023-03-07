using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllInOneApp.Helper
{
    public class TokenProvider : IAccessTokenProvider
    {
        private Func<string[], Task<string>> getTokenDelegate;
        private string[] scopes;

        public TokenProvider(Func<string[], Task<string>> getTokenDelegate, string[] scopes)
        {
            this.getTokenDelegate = getTokenDelegate;
            this.scopes = scopes;
        }

        public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = default,
            CancellationToken cancellationToken = default)
        {
            return getTokenDelegate(scopes);
        }

        public AllowedHostsValidator AllowedHostsValidator { get; }
    }
}
