using ConsistentHashingApi.Infrastructure;
using ConsistentHashingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsistentHashingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var tenantId = HttpContext.Request.Headers["X-TenantId"].FirstOrDefault();
            var products = await _dbContext.Products.AsNoTracking().Where(p => p.TenantId == new Guid(tenantId)).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            var tenantId = HttpContext.Request.Headers["X-TenantId"].FirstOrDefault();
            product.TenantId = new Guid(tenantId);
            product.Id = Guid.NewGuid();
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }
    }
}
