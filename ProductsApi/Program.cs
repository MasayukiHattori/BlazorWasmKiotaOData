using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ProductsApi.Configuration;
using ProductsApi.Data;
using ProductsApi.Identity;
using ProductsApi.Routing;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// DBコンテキストの構成
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=products.db");
});

// Identityを構成するためのサービスを追加
builder.Services.AddAuthorization();

// Identityを構成する
builder.Services.AddCustomIdentityApiEndpoints<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// コントローラーを構成する
builder.Services.AddControllers()
    .AddOData(options =>
    {
        options.EnableQueryFeatures(null);
        options.TimeZone = TimeZoneInfo.Local;
    });

// EDMモデルの構成
builder.Services.AddTransient<IModelConfiguration, ProductModelConfiguration>();

// ODataのバージョン管理を設定
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddOData(options => options.AddRouteComponents("odata"))
.AddODataApiExplorer(options => options.GroupNameFormat = "'v'VVV");

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();

// Swaggerの設定
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    var fileName = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

    // integrate xml comments
    options.IncludeXmlComments(filePath);

    // DocumentFilterを追加
    options.DocumentFilter<BearerSecurityDocumentFilter>();
});

// CORSの設定を追加
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// IdntityのAPIをマッピング
app.MapCustomIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });

    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

internal sealed class BearerSecurityDocumentFilter : IDocumentFilter
{
    private static readonly HashSet<string> _excludedPaths = new()
    {
        "/register",
        "/login",
        "/refresh",
        "/confirmEmail",
        "/resendConfirmationEmail",
        "/forgotPassword",
        "/resetPassword",
    };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // セキュリティスキームが未登録なら追加する
        if (swaggerDoc.Components.SecuritySchemes == null ||
           !swaggerDoc.Components.SecuritySchemes.ContainsKey("Identity.Bearer"))
        {
            swaggerDoc.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
            swaggerDoc.Components.SecuritySchemes["Identity.Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token",
                Name = "Authorization",
                Description = "JWT認証トークンを 'Bearer {token}' の形式で入力してください。"
            };
        }

        foreach (var (path, pathItem) in swaggerDoc.Paths)
        {
            // 特定のパスを除外する
            if (_excludedPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }
            // 各操作にセキュリティ要件を追加
            foreach (var operation in pathItem.Operations)
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Identity.Bearer"
                        }
                    }] = new List<string>()
                });
            }
        }
    }
}