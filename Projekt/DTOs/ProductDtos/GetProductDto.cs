using Projekt.Entities.ProductModels;

namespace Projekt.DTOs.ProductDtos
{
    public class GetProductDto
    {
        public int Id { get; set; }
        public string?   Name { get; set; }
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public decimal CurrentPrice { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        public bool IsDeleted { get; set; }

    }
}
