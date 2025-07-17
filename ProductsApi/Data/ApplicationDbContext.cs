using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Data
{
    /// <summary>
    /// アプリケーションのデータベースコンテキストを表します。
    /// ASP.NET Core Identity の機能を統合しています。
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// <see cref="ApplicationDbContext"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="options">データベースコンテキストの構成オプション。</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 製品情報を格納するデータベースセット。
        /// </summary>
        public DbSet<Product> Products { get; set; } = default!;
    }
}
