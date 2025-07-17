using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProductsApi.Configuration
{
    /// <summary>
    /// SwaggerGenOptions の設定を構成し、すべての API バージョンに対して Swagger ドキュメントを生成します。
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// <see cref="ConfigureSwaggerOptions"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="provider">API バージョン記述プロバイダー。</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// SwaggerGenOptions を構成し、すべての API バージョンに対して Swagger ドキュメントを追加します。
        /// </summary>
        /// <param name="options">SwaggerGenOptions インスタンス。</param>
        public void Configure(SwaggerGenOptions options)
        {
            // すべてのAPIバージョンに対してSwaggerドキュメントを生成
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new()
                {
                    Title = "Products API",
                    Version = description.ApiVersion.ToString(),
                    Description = "Products API for managing product records.",
                });
            }
        }
    }
}
