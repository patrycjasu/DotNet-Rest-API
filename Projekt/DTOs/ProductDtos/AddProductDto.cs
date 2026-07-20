namespace Projekt.DTOs.ProductDtos
{
    public class AddProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public decimal CurrentPrice { get; set; }
        public int CategoryId { get; set; }
    }
}
