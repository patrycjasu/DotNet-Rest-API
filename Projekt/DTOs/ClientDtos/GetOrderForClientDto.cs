namespace Projekt.DTOs.ClientDtos
{
    public class GetOrderForClientDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalWeight { get; set; }
        public IEnumerable<GetProductForOrdersForClientDto> GetProductsForOrdersForClient { get; set; } = new List<GetProductForOrdersForClientDto>();
    }
}
