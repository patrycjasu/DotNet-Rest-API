using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.DTOs.ProductDtos;
using Projekt.Entities.ProductModels;
using Projekt.Exceptions;
using Projekt.Services.Interfaces;

namespace Projekt.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ProductService> _logger;
        public ProductService(AppDbContext dbContext, ILogger<ProductService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task AddProduct(AddProductDto dto, CancellationToken cancellation)
        {
            _logger.LogInformation("Adding product");
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == dto.CategoryId, cancellation);
            if (category == null)
            {
                _logger.LogWarning("Category with id {id} not found", dto.CategoryId);
                throw new NotFoundException("Nie znaleziono kategorii");
            }
            var product = new Product()
            {
                Name = dto.Name!,
                Description = dto.Description,
                Weight = dto.Weight,
                CurrentPrice = dto.CurrentPrice,
                IsDeleted = false,
                Category = category,
                
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellation);
            _logger.LogInformation("Product added with id {id}", product.Id);

        }

        public async Task DeleteProduct(int id, CancellationToken cancellation)
        {
            _logger.LogInformation("Deleting product with id {id}", id);
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (product == null)
            {
                _logger.LogWarning("Product with id {id} not found", id);
                throw new NotFoundException("Nie znaleziono produktu");
            } else if (product.IsDeleted == true)
            {
                _logger.LogWarning("Cannot delete deleted product");
                throw new BadRequestException("Nie mozna usunac usunietego produktu");
            }

            product.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellation);
            _logger.LogInformation("Product deleted");
        }

        public async Task<IEnumerable<GetProductDto>> GetAllProducts(string? name, CancellationToken cancellation)
        {
            _logger.LogInformation("Getting products with name {name}", name);

            var res = await _dbContext.Products.Include(x=> x.Category).Where(x => name == null ||(name != null && x.Name.ToLower().Contains(name.ToLower()))).Select(x => new GetProductDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Weight = x.Weight,
                CurrentPrice = x.CurrentPrice,
                CategoryDescription = x.Category.Description,
                IsDeleted = x.IsDeleted,
                CategoryName = x.Category.Name,

            }).ToListAsync(cancellation);
            return res;
        }

        public async Task<GetProductDto> GetProductById(int id, CancellationToken cancellation)
        {
            _logger.LogInformation("Getting product with id {id}", id);
            var product = await _dbContext.Products.Include(a=> a.Category).FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (product == null)
            {
                _logger.LogWarning("Product with id {id} not found", id);
                throw new NotFoundException("Nie znaleziono produktu");
            }
            var res = new GetProductDto
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Weight = product.Weight,
                CurrentPrice = product.CurrentPrice,
                CategoryName = product.Category.Name,
                CategoryDescription = product.Category.Description,
                IsDeleted = product.IsDeleted
            };
            return res;
        }

        public async Task<IEnumerable<GetProductDto>> GetProductsByCategory(string categoryName, CancellationToken cancellation)
        {
            _logger.LogInformation("Getting products from category {category}", categoryName);
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name == categoryName, cancellation);
            if (category == null)
            {
                _logger.LogWarning("Category not found");
                throw new NotFoundException("Nie znaleziono kategorii");
            }

            var res = await _dbContext.Products.Include(x=>x.Category).Where(x => x.Category.Name == categoryName).Select(x => new GetProductDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description= x.Description,
                Weight= x.Weight,
                CurrentPrice= x.CurrentPrice,
                CategoryDescription = category.Description,
                IsDeleted = x.IsDeleted,
                CategoryName = categoryName,
             
            }).ToListAsync(cancellation);
            return res;
        }

        public async Task UpdateProduct(UpdateProductDto dto, int id, CancellationToken cancellation)
        {
            _logger.LogInformation("Updating product with id {id}", id);
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellation);
            if (product == null)
            {
                _logger.LogWarning("Product with id {id} not found", id);
                throw new NotFoundException("Nie znaleziono produktu");
            }
            else if (product.IsDeleted == true)
            {
                _logger.LogWarning("Cannot update deleted product");
                throw new BadRequestException("Nie mozna zmienci usunietego produktu");
            }

            product.Description = dto.Description;
            product.CurrentPrice = dto.CurrentPrice;

            await _dbContext.SaveChangesAsync(cancellation);
            _logger.LogInformation("Product updated");
        }
    }
}
