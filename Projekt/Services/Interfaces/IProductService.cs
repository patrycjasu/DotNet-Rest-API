using Projekt.DTOs.ProductDtos;

namespace Projekt.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GetProductDto>> GetAllProducts(string? name, CancellationToken cancellation);
        Task<GetProductDto> GetProductById(int id, CancellationToken cancellation);
        Task<IEnumerable<GetProductDto>> GetProductsByCategory(string category, CancellationToken cancellation);
        Task AddProduct(AddProductDto dto, CancellationToken cancellation);
        Task UpdateProduct(UpdateProductDto dto, int id, CancellationToken cancellation);
        Task DeleteProduct(int id, CancellationToken cancellation);
    }
}
