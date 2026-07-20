using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.DTOs.OrderDtos;
using Projekt.Entities.OrderModels;
using Projekt.Exceptions;
using Projekt.Services.Interfaces;

namespace Projekt.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;
        public OrderService(AppDbContext dbContext, ILogger<OrderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;

        }
        public async Task AddOrder(AddOrderDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new order");

            var clientExists = await _dbContext.Clients.FirstOrDefaultAsync(x=>x.Id == dto.ClientId ,cancellationToken);
            if (clientExists == null)
            {
                _logger.LogWarning($"Client with id {dto.ClientId} doesnt exist");
                throw new NotFoundException("Nie znaleziono podanego klienta");
            }

            var order = new Order()
            {
                Date = DateTime.Now,
                ClientId = dto.ClientId,
                Client = clientExists,
                Status = Enums.State.ACTIVE,
                IsDeleted = false,
                OrderProducts = new List<OrderProduct>()
            };

            var totalWeight = decimal.Zero;
            var totalPrice = decimal.Zero;
           

            foreach (var element in dto.ProductsOrders!)
            {
                if (element != null)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == element.ProductId, cancellationToken);
                    if (product != null)
                    {
                        var productOrder = new OrderProduct()
                        {
                            Product = product,
                            Order = order,
                            Quantity = element.Quantity,
                            UnitPrice = product.CurrentPrice
                        };
                        totalPrice += product.CurrentPrice*element.Quantity;
                        totalWeight += product.Weight*element.Quantity;

                        order.OrderProducts.Add(productOrder);
                        _logger.LogInformation($"New product with id {product.Id} added to the order");
                    } else
                    {
                        _logger.LogWarning($"Product with id {element.ProductId} doesnt exist.");
                        throw new NotFoundException("Nie znaleziono takiego produktu");
                    } 
                }
            }

            order.TotalPrice = totalPrice;
            order.TotalWeight = totalWeight;

            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(x => x.MaxWeight >= totalWeight, cancellationToken);

            if (shipment == null)
            {
                _logger.LogWarning("No shipment method available for weight {totalWeight}", totalWeight);
                throw new BadRequestException("Nie ma opcji przewozu dla takiej paczki");
            }

            order.Shipment = shipment;

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Order created with {order.Id}", order.Id);
        }

        public async Task DeleteOrder(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting order with id {id}", id);

            var orderExists = await _dbContext.Orders.FirstOrDefaultAsync(x=> x.Id == id,cancellationToken);
            if (orderExists == null)
            {
                _logger.LogWarning("order with this id not found");
                throw new NotFoundException("Nie znaleziono zamowienia do usuniecia");
            } else if (orderExists.Status == Enums.State.PAID || orderExists.IsDeleted == true)
            {
                _logger.LogWarning("cant delete this order");
                throw new BadRequestException("Nie mozna usunac oplaconego zamowienia lub usunietego");
            }

            orderExists.IsDeleted = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Order with id {id} deleted", id); ;
        }

        public async Task<IEnumerable<GetOrderDto>> GetAllOrders(bool? active, CancellationToken cancellationToken)
        {
            var orders = new List<GetOrderDto>();
            _logger.LogInformation("Getting orders");
            if (active == true)
            {
                orders = await _dbContext.Orders.Include(a => a.Payments).Include(a => a.OrderProducts).Where(x=> x.Status==Enums.State.ACTIVE).Select(x => new GetOrderDto()
                {

                    Id = x.Id,
                    TotalPrice = x.TotalPrice,
                    TotalWeight = x.TotalWeight,
                    ClientId = x.ClientId,
                    Date = x.Date,
                    Status = x.Status,
                    IsDeleted = x.IsDeleted,
                    ShipmentName = x.Shipment.Name,
                    Payments = x.Payments.Select(a => new DTOs.PaymentDtos.GetPaymentDto()
                    {
                        Amount = a.Amount,
                        Date = a.Date,
                        Method = a.Method
                    }).ToList(),

                    ProductsOrders = x.OrderProducts.Select(a => new GetProductInOrderDto()
                    {
                        Name = a.Product.Name,
                        Description = a.Product.Description,
                        Weight = a.Product.Weight,
                        Quantity = a.Quantity,
                        UnitPrice = a.UnitPrice,
                        CategoryName = a.Product.Category.Name
                    }).ToList()

                }).ToListAsync(cancellationToken);
            }
            else
            {
                orders = await _dbContext.Orders.Include(a=> a.Payments).Include(a=>a.OrderProducts).Select(x => new GetOrderDto()
                {

                    Id = x.Id,
                    TotalPrice = x.TotalPrice,
                    TotalWeight = x.TotalWeight,
                    ClientId = x.ClientId,
                    Date = x.Date,
                    Status = x.Status,
                    IsDeleted = x.IsDeleted,
                    ShipmentName = x.Shipment.Name,
                    Payments = x.Payments.Select(a => new DTOs.PaymentDtos.GetPaymentDto()
                    {
                        Amount= a.Amount,
                        Date= a.Date,
                        Method = a.Method
                    }).ToList(),

                    ProductsOrders = x.OrderProducts.Select(a => new GetProductInOrderDto()
                    {
                        Name = a.Product.Name,
                        Description = a.Product.Description,
                        Weight = a.Product.Weight,
                        Quantity = a.Quantity,
                        UnitPrice = a.UnitPrice,
                        CategoryName = a.Product.Category.Name
                    }).ToList()

                }).ToListAsync(cancellationToken);
            }

            return orders;
        }

        public async Task<GetOrderDto> GetOrderById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting order with id {id}", id);
            var order = await _dbContext.Orders
                .Include(x => x.OrderProducts)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Category)
                .Include(x=> x.Payments)
                .Include(x=> x.Shipment)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (order == null)
            {
                _logger.LogWarning("order with this id not found");
                throw new NotFoundException("Nie znaleziono zamowienia");
            }
 

            var res = new GetOrderDto()
            {
                Id = id,
                TotalPrice = order.TotalPrice,
                TotalWeight = order.TotalWeight,
                ClientId = order.ClientId,
                ShipmentName = order.Shipment.Name,
                Date = order.Date,
                Status = order.Status,
                IsDeleted = order.IsDeleted,
                ProductsOrders = order.OrderProducts.Select(x => new GetProductInOrderDto()
                {
                    Name = x.Product.Name,
                    Description = x.Product.Description,
                    Weight = x.Product.Weight,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    CategoryName = x.Product.Category.Name,
                }).ToList(),
                Payments = order.Payments.Select(x => new DTOs.PaymentDtos.GetPaymentDto()
                {
                    Method = x.Method,
                    Amount = x.Amount,
                    Date = x.Date
                }).ToList()
            };

            return res;

        }

        public async Task UpdateOrder(UpdateOrderDto dto, int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating order with id {id}", id);

            var order = await _dbContext.Orders.Include(x=> x.OrderProducts).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (order == null)
            {
                _logger.LogWarning("order with this id not found");
                throw new NotFoundException("Nie znaleziono zamowienia do zmiany");
            }
            else if (order.Status == Enums.State.PAID || order.IsDeleted == true)
            {
                _logger.LogWarning("cant update this order");
                throw new BadRequestException("Nie mozna zmienic oplaconego zamowienia lub usunietego");
            }

            if (dto.ProductsOrders != null)
            {
                _dbContext.OrderProducts.RemoveRange(order.OrderProducts);
                order.OrderProducts.Clear();

                var totalWeight = decimal.Zero;
                var totalPrice = decimal.Zero;

                foreach (var element in dto.ProductsOrders)
                {
                    if (element != null)
                    {
                        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == element.ProductId, cancellationToken);
                        if (product != null)
                        {

                            var productOrder = new OrderProduct()
                            {
                                Product = product,
                                Order = order,
                                Quantity = element.Quantity,
                                UnitPrice = product.CurrentPrice
                            };
                            order.OrderProducts.Add(productOrder);

                            totalPrice += product.CurrentPrice * element.Quantity;
                            totalWeight += product.Weight * element.Quantity;


                            _logger.LogInformation($"product with id {product.Id} added to the order");
                        }
                        else
                        {

                            _logger.LogWarning($"Product with id {element.ProductId} doesnt exist.");
                            throw new NotFoundException("Nie znaleziono takiego produktu");
                        }
                    }
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Order with id {id} updated", id); ;

        }
    }
}
