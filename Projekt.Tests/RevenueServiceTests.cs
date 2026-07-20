using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Projekt.Data;
using Projekt.DTOs.OrderDtos;
using Projekt.DTOs.PaymentDtos;
using Projekt.Entities.ClientModels;
using Projekt.Entities.OrderModels;
using Projekt.Entities.ProductModels;
using Projekt.Services;
using Projekt.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Projekt.Tests
{
    public class RevenueServiceTests
    {
        [Fact]
        public async Task GetActualRevenueShouldReturnActualRevenue()
        {
            var logger = new Mock<ILogger<RevenueService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new RevenueService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context, totalPrice:10m, state: Enums.State.ACTIVE, id:1);

            await TestDataSeeder.SeedOrder(context, state: Enums.State.PAID, id:2);

            var result = await service.GetActualRevenue(CancellationToken.None);
            Assert.Equal(6, result);
        }

        [Fact]
        public async Task GetExpectedRevenueShouldReturnExpectedRevenue()
        {
            var logger = new Mock<ILogger<RevenueService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new RevenueService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context, totalPrice: 10m, state: Enums.State.ACTIVE,id:1);
            await TestDataSeeder.SeedOrder(context, state: Enums.State.PAID,id:2);

            await context.SaveChangesAsync();
            var result = await service.GetExpectedRevenue(CancellationToken.None);
            Assert.Equal(16, result);
        }
    }
}
