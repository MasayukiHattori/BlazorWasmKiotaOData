using BlazorWasm.Identity.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BlazorWasm.Identity
{
    public class CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILogger<CookieAuthenticationStateProvider> logger)
        : AuthenticationStateProvider, IAccountManagement, IAuthenticationProvider
    {
        /// <summary>
        /// Map the JavaScript-formatted properties to C#-formatted classes.
        /// </summary>
        private readonly JsonSerializerOptions jsonSerializerOptions =
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        /// <summary>
        /// Authentication state.
        /// </summary>
        private bool authenticated = false;

        /// <summary>
        /// Default principal for anonymous (not authenticated) users.
        /// </summary>
        private readonly ClaimsPrincipal unauthenticated = new(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            authenticated = false;

            // default to not authenticated
            var user = unauthenticated;

            try
            {
                // the user info endpoint is secured, so if the user isn't logged in this will fail
                using var userResponse = await _httpClient.GetAsync("manage/info");

                // throw if user info wasn't retrieved
                userResponse.EnsureSuccessStatusCode();

                // user is authenticated,so let's build their authenticated identity
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, jsonSerializerOptions);

                if (userInfo != null)
                {
                    // in this example app, name and email are the same
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email),
                    };

                    // add any additional claims
                    claims.AddRange(
                        userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                            .Select(c => new Claim(c.Key, c.Value)));

                    // set the principal
                    var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                    user = new ClaimsPrincipal(id);
                    authenticated = true;
                }
            }
            catch (Exception ex) when (ex is HttpRequestException exception)
            {
                if (exception.StatusCode != HttpStatusCode.Unauthorized)
                {
                    logger.LogError(ex, "App error");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "App error");
            }

            // return the state
            return new AuthenticationState(user);
        }

        /// <summary>
        /// 新規ユーザーを登録します。
        /// </summary>
        /// <param name="email">ユーザーのメールアドレス。</param>
        /// <param name="password">ユーザーのパスワード。</param>
        /// <returns>
        /// リクエストの結果を <see cref="FormResult"/> にシリアライズしたもの。
        /// 成功時は <c>Succeeded = true</c>、失敗時は <c>Succeeded = false</c> かつ <c>ErrorList</c> にエラー内容が格納されます。
        /// </returns>
        public async Task<FormResult> RegisterAsync(string email, string password)
        {
            string[] defaultDetail = ["An unknown error prevented registration from succeeding."];

            try
            {
                // make the request
                var result = await _httpClient.PostAsJsonAsync(
                    "register", new
                    {
                        email,
                        password
                    });

                // successful?
                if (result.IsSuccessStatusCode)
                {
                    return new FormResult { Succeeded = true };
                }

                // body should contain details about why it failed
                var details = await result.Content.ReadAsStringAsync();
                var problemDetails = JsonDocument.Parse(details);
                var errors = new List<string>();
                var errorList = problemDetails.RootElement.GetProperty("errors");

                foreach (var errorEntry in errorList.EnumerateObject())
                {
                    if (errorEntry.Value.ValueKind == JsonValueKind.String)
                    {
                        errors.Add(errorEntry.Value.GetString()!);
                    }
                    else if (errorEntry.Value.ValueKind == JsonValueKind.Array)
                    {
                        errors.AddRange(
                            errorEntry.Value.EnumerateArray().Select(
                                e => e.GetString() ?? string.Empty)
                            .Where(e => !string.IsNullOrEmpty(e)));
                    }
                }

                // return the error list
                return new FormResult
                {
                    Succeeded = false,
                    ErrorList = problemDetails == null ? defaultDetail : [.. errors]
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "App error");
            }

            // unknown error
            return new FormResult
            {
                Succeeded = false,
                ErrorList = defaultDetail
            };
        }

        /// <summary>
        /// ログインサービス
        /// </summary>
        /// <param name="email">ユーザーのメールアドレス</param>
        /// <param name="password">ユーザーのパスワード</param>
        /// <returns>リクエストの結果を<see cref="FormResult"/>にシリアライズしたもの</returns>
        public async Task<FormResult> LoginAsync(string email, string password)
        {
            try
            {
                // login with cookies
                var result = await _httpClient.PostAsJsonAsync(
                    "login?useCookies=true", new
                    {
                        email,
                        password
                    });

                // success?
                if (result.IsSuccessStatusCode)
                {
                    // need to refresh auth state
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                    // success!
                    return new FormResult { Succeeded = true };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "App error");
            }

            // unknown error
            return new FormResult
            {
                Succeeded = false,
                ErrorList = ["Invalid email and/or password."]
            };
        }

        /// <summary>
        /// ログアウト
        /// </summary>
        /// <returns>非同期タスク</returns>
        public async Task LogoutAsync()
        {
            const string Empty = "{}";
            var emptyContent = new StringContent(Empty, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("logout", emptyContent);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<bool> CheckAuthenticatedAsync()
        {
            await GetAuthenticationStateAsync();
            return authenticated;
        }

        public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            var authenticated = await CheckAuthenticatedAsync();
            if (!authenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
        }
    }
}
