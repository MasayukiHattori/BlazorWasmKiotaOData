using Microsoft.Kiota.Http.HttpClientLibrary;

namespace BlazorWasm.Client
{
    /// <summary>
    /// Kiota の HTTP ハンドラーを DI コンテナおよび HttpClientBuilder に登録する拡張メソッドを提供します。
    /// </summary>
    public static class KiotaServiceCollectionExtensions
    {
        /// <summary>
        /// Kiota のハンドラーをサービスコレクションに追加します。
        /// </summary>
        /// <param name="services">サービスを追加する <see cref="IServiceCollection"/>。</param>
        /// <returns>慣例に従い、同じ <see cref="IServiceCollection"/> を返します。</returns>
        /// <remarks>
        /// ハンドラーは <see cref="AttachKiotaHandlers(IHttpClientBuilder)"/> の呼び出しによって HTTP クライアントに追加されます。
        /// このため、事前に DI に登録されている必要があります。
        /// </remarks>
        public static IServiceCollection AddKiotaHandlers(this IServiceCollection services)
        {
            // クライアントファクトリから Kiota ハンドラーの型を動的に取得
            var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();
            // DI コンテナにハンドラーを登録
            foreach (var handler in kiotaHandlers)
            {
                services.AddTransient(handler.Type);
            }

            return services;
        }

        /// <summary>
        /// Kiota のハンドラーを HTTP クライアントビルダーに追加します。
        /// </summary>
        /// <param name="builder">ハンドラーを追加する <see cref="IHttpClientBuilder"/>。</param>
        /// <returns>同じ <see cref="IHttpClientBuilder"/> を返します。</returns>
        /// <remarks>
        /// <see cref="AddKiotaHandlers(IServiceCollection)"/> によって事前にハンドラーが DI に登録されている必要があります。
        /// ハンドラーの追加順序は実行順序に影響します。
        /// </remarks>
        public static IHttpClientBuilder AttachKiotaHandlers(this IHttpClientBuilder builder)
        {
            // クライアントファクトリから Kiota ハンドラーの型を動的に取得
            var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();
            // HTTP クライアントビルダーにハンドラーを追加
            foreach (var handler in kiotaHandlers)
            {
                builder.AddHttpMessageHandler((sp) => (DelegatingHandler)sp.GetRequiredService(handler.Type));
            }

            return builder;
        }
    }
}
