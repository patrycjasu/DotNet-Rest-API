namespace Projekt.DTOs.ClientDtos
{
    public class GetProductForOrdersForClientDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }

    }
}
