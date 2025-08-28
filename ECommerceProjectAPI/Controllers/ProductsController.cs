using ECommerceProjectAPI.Data;
using ECommerceProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProjectAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly ProductDBContext _dbContext;

        public ProductsController(ProductDBContext context)
        {
            _dbContext = context;
        }
        [Authorize(Roles ="USER")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        [Authorize(Roles ="Admin,ADMIN")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }
            _dbContext.Entry(product).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                return Conflict(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _dbContext.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
