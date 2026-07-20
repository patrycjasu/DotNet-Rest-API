using Projekt.Entities.ProductModels;

namespace Projekt.Entities.OrderModels
{
    public class OrderProduct
    {
        public int ProductId { get; set; }
        public int OrderId { get; set;  }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set;  }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
