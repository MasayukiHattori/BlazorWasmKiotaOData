using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ProductsApi.Data;
using ProductsApi.Models;

namespace ProductsApi.Controllers.V1
{
    /// <summary>
    /// 製品情報を管理する OData API コントローラー (API バージョン 1.0)。
    /// 製品の取得、追加、更新、削除を提供します。
    /// </summary>
    [ApiVersion("1.0")]
    [Authorize]
    public class ProductsController : ODataController
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// <see cref="ProductsController"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context">アプリケーションのデータベースコンテキスト。</param>
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 製品一覧を取得します。
        /// </summary>
        /// <returns>製品のコレクション</returns>
        [HttpGet]
        [EnableQuery]
        [Produces("application/json")]
        [ProducesResponseType<ODataValue<IEnumerable<Product>>>(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Product>> Get() => Ok(_context.Products);

        /// <summary>
        /// 指定したキーの製品を取得します。
        /// </summary>
        /// <param name="key">製品ID</param>
        /// <returns>製品情報</returns>
        [HttpGet]
        [EnableQuery]
        [Produces("application/json")]
        [ProducesResponseType<Product>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public SingleResult<Product> Get(int key)
            => SingleResult.Create(_context.Products.Where(p => p.Id == key));

        /// <summary>
        /// 新しい製品を追加します。
        /// </summary>
        /// <param name="product">追加する製品情報</param>
        /// <returns>追加結果</returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType<Product>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Post([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            var createdProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            return CreatedAtAction(nameof(Get), new { key = createdProduct.Id }, createdProduct);
        }

        /// <summary>
        /// 指定した製品情報を部分更新します。
        /// </summary>
        /// <param name="key">製品ID</param>
        /// <param name="delta">更新内容</param>
        /// <returns>更新結果</returns>
        [HttpPatch]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType<Product>(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Patch(int key, [FromBody] Delta<Product> delta)
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == key);
            if (existingProduct == null)
            {
                return NotFound();
            }

            delta.Patch(existingProduct);

            if (!TryValidateModel(existingProduct))
            {
                return BadRequest(ModelState);
            }

            delta.Patch(existingProduct);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// 指定した製品を削除します。
        /// </summary>
        /// <param name="key">製品ID</param>
        /// <returns>削除結果</returns>
        [HttpDelete]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int key)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == key);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
