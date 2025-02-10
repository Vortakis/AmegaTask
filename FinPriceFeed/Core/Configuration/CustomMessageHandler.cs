using System.Web;

namespace FinPriceFeed.Core.Configuration
{
    public class CustomMessageHandler : DelegatingHandler
    {
        private readonly string _authType;
        private readonly string _apiKey;

        public CustomMessageHandler(string authType, string apiKey)
        {
            _authType = authType;
            _apiKey = apiKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder(request.RequestUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[_authType] = _apiKey;
            uriBuilder.Query = query.ToString();
            request.RequestUri = uriBuilder.Uri;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
