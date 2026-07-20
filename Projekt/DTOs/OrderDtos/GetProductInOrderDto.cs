namespace Projekt.DTOs.OrderDtos
{
    public class GetProductInOrderDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? CategoryName { get; set; }
    }
}
