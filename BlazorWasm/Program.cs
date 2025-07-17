using BlazorWasm;
using BlazorWasm.Client;
using BlazorWasm.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Kiota.Abstractions.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// register the cookie handler
builder.Services.AddTransient<CookieHandler>();

// set up authorization
builder.Services.AddAuthorizationCore();

// register the custom state provider
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

// register the account management interface
builder.Services.AddScoped(
    sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

// register the authentication provider interface
builder.Services.AddScoped(
    sp=>(IAuthenticationProvider)sp.GetRequiredService<AuthenticationStateProvider>());

// add Kiota handlers
builder.Services.AddKiotaHandlers();

// configure client for auth interactions
builder.Services.AddHttpClient(
    "Auth",
    opt => opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001"))
    .AddHttpMessageHandler<CookieHandler>()
    .AttachKiotaHandlers();

builder.Services.AddScoped<ApiClientFactory>();

builder.Services.AddTransient(sp=>sp.GetRequiredService<ApiClientFactory>().GetClient());

await builder.Build().RunAsync();
