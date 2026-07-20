using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Projekt.Data;
using Projekt.DTOs.OrderDtos;
using Projekt.Entities.ClientModels;
using Projekt.Entities.OrderModels;
using Projekt.Entities.ProductModels;
using Projekt.Exceptions;
using Projekt.Services;
using Projekt.Tests.Helpers;
using System;

namespace Projekt.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task AddOrderShouldAddOrder()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedProduct(context);
            await TestDataSeeder.SeedClient(context);
            await TestDataSeeder.SeedShipment(context);

            var dto = new AddOrderDto
            {
                ClientId = 1,
                ProductsOrders = [
                    new AddUpdateProductInOrderDto
                    {
                        ProductId = 1,
                        Quantity = 3
                    }]
            };

            await service.AddOrder(dto, CancellationToken.None);
            var order = await context.Orders.FirstOrDefaultAsync();

            Assert.NotNull(order);
            Assert.Equal(1, order.ClientId);
            Assert.Equal(1, order.Id);
            Assert.False(order.IsDeleted);
            Assert.Equal(Enums.State.ACTIVE, order.Status);
            Assert.Equal(1, order.ShipmentId);
            Assert.Equal(new decimal(6), order.TotalPrice);
            Assert.Equal(new decimal(6), order.TotalWeight);
        }

        [Fact]
        public async Task AddOrderWithWrongProductIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedClient(context);
            await TestDataSeeder.SeedShipment(context);

            await context.SaveChangesAsync();

            var dto = new AddOrderDto
            {
                ClientId = 1,
                ProductsOrders = [new AddUpdateProductInOrderDto
                {
                    ProductId = 1,
                    Quantity = 3
                }]
            };
            await Assert.ThrowsAsync<NotFoundException>(() => service.AddOrder(dto, CancellationToken.None));
        }

        [Fact]
        public async Task AddOrderWithWrongClientIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);


            await TestDataSeeder.SeedProduct(context);
            await TestDataSeeder.SeedShipment(context);

            var dto = new AddOrderDto
            {
                ClientId = 1,
                ProductsOrders = 
                    [new AddUpdateProductInOrderDto
                        {
                            ProductId = 1,
                            Quantity = 3
                        }]
            };
            await Assert.ThrowsAsync<NotFoundException>(() => service.AddOrder(dto, CancellationToken.None));
        }

        
        [Fact]
        public async Task AddOrderWithNoShipmentShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedProduct(context);
            await TestDataSeeder.SeedClient(context);

            var dto = new AddOrderDto
            {
                ClientId = 1,
                ProductsOrders = [new AddUpdateProductInOrderDto
                    {
                        ProductId = 1,
                        Quantity = 3
                    }],
            };

            await Assert.ThrowsAsync<BadRequestException>(() => service.AddOrder(dto, CancellationToken.None)); ;
        }

        [Fact]
        public async Task DeleteOrderShouldDeleteOrder()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context);

            await service.DeleteOrder(1, CancellationToken.None);
            var order = await context.Orders.FirstOrDefaultAsync();
            Assert.NotNull(order);
            Assert.True(order.IsDeleted);

        }

        [Fact]
        public async Task DeleteOrderWithWrongIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteOrder(1, CancellationToken.None)); ;
        }

        [Fact]
        public async Task DeleteOrderAlreadyPaidShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context, state: Enums.State.PAID);

            await Assert.ThrowsAsync<BadRequestException>(() => service.DeleteOrder(1, CancellationToken.None)); ;
        }

        [Fact]
        public async Task DeleteOrderAlreadyDeletedShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context, isDeleted:true);

            await Assert.ThrowsAsync<BadRequestException>(() => service.DeleteOrder(1, CancellationToken.None)); ;
        }

        [Fact]
        public async Task GetAllOrdersShouldReturnListOfAllOrders()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrderWithProducts(context, state: Enums.State.ACTIVE, id:1);
            await TestDataSeeder.SeedOrderWithProducts(context, state: Enums.State.PAID, id:2);

            var orders = await service.GetAllOrders(null, CancellationToken.None);
            Assert.NotNull(orders);
            Assert.Equal(2, orders.Count());

        }

        [Fact]
        public async Task GetAllOrdersActiveShouldReturnListOfActiveOrders()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrderWithProducts(context, state: Enums.State.ACTIVE, id:1);
            await TestDataSeeder.SeedOrderWithProducts(context, state: Enums.State.PAID, id: 2);

            var orders = await service.GetAllOrders(true, CancellationToken.None);
            Assert.NotNull(orders);
            Assert.Single(orders);
        }

        [Fact]
        public async Task GetAllOrdersShouldReturnEmptyList()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            var orders = await service.GetAllOrders(true, CancellationToken.None);
            Assert.NotNull(orders);
            Assert.Empty(orders);
        }

        [Fact]
        public async Task GetOrderByIdShouldReturnAnOrder()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrderWithProducts(context);

            var order = await service.GetOrderById(1, CancellationToken.None);
            Assert.NotNull(order);
            Assert.Equal(new decimal(6), order.TotalWeight);
            Assert.Equal(new decimal(6), order.TotalPrice);
            Assert.Equal(1, order.ClientId);
            Assert.Equal(Enums.State.ACTIVE, order.Status);
            Assert.False(order.IsDeleted);
        }

        [Fact]
        public async Task GetOrderByIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteOrder(1, CancellationToken.None)); ;

        }

        [Fact]
        public async Task UpdateOrderShouldUpdateOrder()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context);

            var dto = new UpdateOrderDto
            {
                ProductsOrders = new List<AddUpdateProductInOrderDto>
                { 
                    new AddUpdateProductInOrderDto()
                    {
                        ProductId = 1,
                        Quantity = 2
                    } 
                }
            };

            await service.UpdateOrder(dto, 1, CancellationToken.None);
            var order = context.Orders.FirstOrDefault();
            Assert.NotNull(order);
            Assert.False(order.IsDeleted);
            Assert.Equal(new decimal(6), order.TotalPrice);
            Assert.Equal(new decimal(6), order.TotalWeight);
            Assert.Equal(Enums.State.ACTIVE, order.Status);
            Assert.Equal(1, order.ClientId);
            Assert.Equal(1, order.ShipmentId);
            Assert.Single(order.OrderProducts);
            Assert.NotNull(order.OrderProducts.FirstOrDefault());
            Assert.Equal(2, order.OrderProducts.FirstOrDefault()!.Quantity);
        }

        [Fact]
        public async Task UpdateOrderWithWrongIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            var dto = new UpdateOrderDto
            {
                ProductsOrders = new List<AddUpdateProductInOrderDto>
                {
                    new AddUpdateProductInOrderDto()
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateOrder(dto, 1, CancellationToken.None)); ;
        }
               
        [Fact]
        public async Task UpdateOrderWithPaidStatusShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrderWithProducts(context, state: Enums.State.PAID);

            var dto = new UpdateOrderDto
            {
                ProductsOrders = new List<AddUpdateProductInOrderDto>
                {
                    new AddUpdateProductInOrderDto()
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };
            await Assert.ThrowsAsync<BadRequestException>(() => service.UpdateOrder(dto, 1, CancellationToken.None));
        }
        [Fact]
        public async Task UpdateOrderWithDeletedStatusShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrderWithProducts(context, isDeleted: true);

            var dto = new UpdateOrderDto
            {
                ProductsOrders = new List<AddUpdateProductInOrderDto>
                {
                    new AddUpdateProductInOrderDto()
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };
            await Assert.ThrowsAsync<BadRequestException>(() => service.UpdateOrder(dto, 1, CancellationToken.None));
        }
        [Fact]
        public async Task UpdateOrderWithWrongProductIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<OrderService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new OrderService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context);

            var dto = new UpdateOrderDto
            {
                ProductsOrders = new List<AddUpdateProductInOrderDto>
                {
                    new AddUpdateProductInOrderDto()
                    {
                        ProductId = 5,
                        Quantity = 2
                    }
                }
            };
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateOrder(dto, 1, CancellationToken.None)); ;
        }
    }
}
