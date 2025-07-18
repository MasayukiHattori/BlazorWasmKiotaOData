// <auto-generated/>
#pragma warning disable CS0618
using BlazorWasm.Client.Models;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace BlazorWasm.Client.Login
{
    /// <summary>
    /// Builds and executes requests for operations under \login
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class LoginRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::BlazorWasm.Client.Login.LoginRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public LoginRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/login{?useCookies*,useSessionCookies*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BlazorWasm.Client.Login.LoginRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public LoginRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/login{?useCookies*,useSessionCookies*}", rawUrl)
        {
        }
        /// <summary>
        /// ユーザーIDとパスワードを使用してユーザーにログインします。
        /// </summary>
        /// <returns>A <see cref="global::BlazorWasm.Client.Models.AccessTokenResponse"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::BlazorWasm.Client.Models.HttpValidationProblemDetails">When receiving a 400 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::BlazorWasm.Client.Models.AccessTokenResponse?> PostAsync(global::BlazorWasm.Client.Models.LoginRequest body, Action<RequestConfiguration<global::BlazorWasm.Client.Login.LoginRequestBuilder.LoginRequestBuilderPostQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::BlazorWasm.Client.Models.AccessTokenResponse> PostAsync(global::BlazorWasm.Client.Models.LoginRequest body, Action<RequestConfiguration<global::BlazorWasm.Client.Login.LoginRequestBuilder.LoginRequestBuilderPostQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "400", global::BlazorWasm.Client.Models.HttpValidationProblemDetails.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::BlazorWasm.Client.Models.AccessTokenResponse>(requestInfo, global::BlazorWasm.Client.Models.AccessTokenResponse.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// ユーザーIDとパスワードを使用してユーザーにログインします。
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPostRequestInformation(global::BlazorWasm.Client.Models.LoginRequest body, Action<RequestConfiguration<global::BlazorWasm.Client.Login.LoginRequestBuilder.LoginRequestBuilderPostQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToPostRequestInformation(global::BlazorWasm.Client.Models.LoginRequest body, Action<RequestConfiguration<global::BlazorWasm.Client.Login.LoginRequestBuilder.LoginRequestBuilderPostQueryParameters>> requestConfiguration = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            requestInfo.SetContentFromParsable(RequestAdapter, "application/json", body);
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::BlazorWasm.Client.Login.LoginRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::BlazorWasm.Client.Login.LoginRequestBuilder WithUrl(string rawUrl)
        {
            return new global::BlazorWasm.Client.Login.LoginRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// ユーザーIDとパスワードを使用してユーザーにログインします。
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class LoginRequestBuilderPostQueryParameters 
        {
            [QueryParameter("useCookies")]
            public bool? UseCookies { get; set; }
            [QueryParameter("useSessionCookies")]
            public bool? UseSessionCookies { get; set; }
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class LoginRequestBuilderPostRequestConfiguration : RequestConfiguration<global::BlazorWasm.Client.Login.LoginRequestBuilder.LoginRequestBuilderPostQueryParameters>
        {
        }
    }
}
#pragma warning restore CS0618
