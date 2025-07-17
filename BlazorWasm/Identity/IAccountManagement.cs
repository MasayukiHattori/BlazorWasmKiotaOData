using BlazorWasm.Identity.Models;

namespace BlazorWasm.Identity
{
    /// <summary>
    /// アカウント管理サービスのインターフェースです。
    /// </summary>
    public interface IAccountManagement
    {
        /// <summary>
        /// ログインサービスを提供します。
        /// </summary>
        /// <param name="email">ユーザーのメールアドレス。</param>
        /// <param name="password">ユーザーのパスワード。</param>
        /// <returns>
        /// リクエストの結果を <see cref="FormResult"/> にシリアライズしたもの。
        /// 成功時は <c>Succeeded = true</c>、失敗時は <c>Succeeded = false</c> かつ <c>ErrorList</c> にエラー内容が格納されます。
        /// </returns>
        public Task<FormResult> LoginAsync(string email, string password);

        /// <summary>
        /// ログイン中のユーザーをログアウトします。
        /// </summary>
        /// <returns>非同期タスク。</returns>
        public Task LogoutAsync();

        /// <summary>
        /// 新規ユーザーを登録します。
        /// </summary>
        /// <param name="email">ユーザーのメールアドレス。</param>
        /// <param name="password">ユーザーのパスワード。</param>
        /// <returns>
        /// リクエストの結果を <see cref="FormResult"/> にシリアライズしたもの。
        /// 成功時は <c>Succeeded = true</c>、失敗時は <c>Succeeded = false</c> かつ <c>ErrorList</c> にエラー内容が格納されます。
        /// </returns>
        public Task<FormResult> RegisterAsync(string email, string password);

        /// <summary>
        /// 現在のユーザーが認証されているかどうかを確認します。
        /// </summary>
        /// <returns>認証済みの場合は <c>true</c>、それ以外は <c>false</c>。</returns>
        public Task<bool> CheckAuthenticatedAsync();
    }
}
