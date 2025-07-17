using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace BlazorWasm.Client
{
    public class ApiClientFactory
    {
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly HttpClient _httpClient;

        public ApiClientFactory(IHttpClientFactory httpClientFactory, IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
            _httpClient = httpClientFactory.CreateClient("Auth");
        }

        public ApiClient GetClient()
        {
            return new ApiClient(new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient));
        }
    }
}
