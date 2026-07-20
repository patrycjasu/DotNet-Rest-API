using Projekt.Entities.OrderModels;

namespace Projekt.Entities.ProductModels
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public decimal CurrentPrice { get; set; }
        public int CategoryId { get; set; }
        public bool IsDeleted { get; set; }
        public Category Category { get; set; }
        public IEnumerable<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    }
}
