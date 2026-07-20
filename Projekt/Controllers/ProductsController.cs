using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt.DTOs.ProductDtos;
using Projekt.Services.Interfaces;

namespace Projekt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        public ProductsController(IProductService productService) {

            _productService = productService;

        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody]AddProductDto dto, CancellationToken cancellationToken)
        {
            await _productService.AddProduct(dto, cancellationToken);
            return Created();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task <IActionResult> DeleteProduct([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _productService.DeleteProduct(id, cancellationToken);
            return NoContent();
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery] string? name, CancellationToken cancellationToken)
        {
            var res = await _productService.GetAllProducts(name, cancellationToken);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await  _productService.GetProductById(id, cancellationToken);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("category")]
        public async Task<ActionResult> GetProductsByCategory([FromQuery] string category, CancellationToken cancellationToken)
        {
            var res = await _productService.GetProductsByCategory(category, cancellationToken);
            return Ok(res);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
        {
            await _productService.UpdateProduct(dto, id, cancellationToken);
            return NoContent() ;
        }

    }
}
