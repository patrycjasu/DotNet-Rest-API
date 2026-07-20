using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.CodeCoverage;
using Projekt.Data;
using Projekt.Entities.ClientModels;
using Projekt.Entities.OrderModels;
using Projekt.Entities.ProductModels;
using Projekt.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Projekt.Tests.Helpers
{
    public static class TestDataSeeder
    {
        public static async Task<PrivatePerson> SeedClient(AppDbContext dbContext, int id = 1, string firstName = "John", string lastName= "Doe", string pesel = "12345678901")
        {
            var client = new PrivatePerson
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = "johndoe@mail.com",
                Phone = "987654321",
                Address = "example address",
                PESEL = pesel
            };

            dbContext.Clients.Add(client);
            await dbContext.SaveChangesAsync();
            return client;
        }

        public static async Task<Company> SeedClientCompany(AppDbContext dbContext,int id = 1, string name = "Company", string nip = "1234567890")
        {
            var client = new Company
            {
                Id = id,
                Name = name, 
                Email = "company@mail.com",
                Phone = "987654321",
                Address = "example address",
                NIP = nip
            };

            dbContext.Clients.Add(client);
            await dbContext.SaveChangesAsync();
            return client;
        }

        public static async Task<Category> SeedCategory(AppDbContext dbContext)
        {

            var cat = new Category
            {
                Id = 1,
                Name = "Category"
            };
            dbContext.Categories.Add(cat);
            await dbContext.SaveChangesAsync();
            return cat;
        }

        public static async Task<Shipment> SeedShipment(AppDbContext dbContext)
        {
            var ship = new Shipment
            {
                Id = 1,
                Name = "B",
                MaxWeight = new decimal(100),
            };

            dbContext.Shipments.Add(ship);
            await dbContext.SaveChangesAsync();
            return ship;
        }
        public static async Task<Product> SeedProduct(AppDbContext dbContext, bool isDeleted = false, string name = "A", int id = 1)
        {
            if (!dbContext.Categories.Any())
            {
                await SeedCategory(dbContext);
            }
            var product = new Product
            {
                Id = id,
                Name = name,
                Weight = new decimal(2),
                CurrentPrice = new decimal(2),
                CategoryId = 1,
                IsDeleted = isDeleted
            };
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
            return product;
        }

        public static async Task<Order> SeedOrder(AppDbContext dbContext, Enums.State state = Enums.State.ACTIVE, bool isDeleted = false, decimal totalPrice = 6m, int id = 1)
        {
            if (!dbContext.Clients.Any())
            {
                await SeedClient(dbContext);
            }
            if (!dbContext.Shipments.Any())
            {
                await SeedShipment(dbContext);
            }
            if (!dbContext.Products.Any())
            {
                await SeedProduct(dbContext);
            }
            var order = new Order
            {
                Id = id,
                TotalPrice = totalPrice,
                TotalWeight = new decimal(6),
                ShipmentId = 1,
                ClientId = 1,
                Date = DateTime.Now,
                Status = state,
                IsDeleted = isDeleted
            };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            return order;
        }
        public static async Task<Order> SeedOrderWithProducts(AppDbContext dbContext, Enums.State state = Enums.State.ACTIVE, bool isDeleted = false, int id = 1)
        {
            if (!dbContext.Clients.Any())
            {
                await SeedClient(dbContext);
            }
            if (!dbContext.Shipments.Any())
            {
                await SeedShipment(dbContext);
            }
            if (!dbContext.Products.Any())
            {
                await SeedProduct(dbContext);
            }
            var order = new Order
            {
                Id = id,
                TotalPrice = new decimal(6),
                TotalWeight = new decimal(6),
                ShipmentId = 1,
                ClientId = 1,
                Date = DateTime.Now,
                Status = state,
                IsDeleted = isDeleted
            };

            var orderProduct = new OrderProduct()
            {
                OrderId = id,
                ProductId = 1,
                Quantity = 3,
                UnitPrice = new decimal(2)
            };

            dbContext.Orders.Add(order);
            dbContext.OrderProducts.Add(orderProduct);
            await dbContext.SaveChangesAsync();
            return order;

        }

    }
}
