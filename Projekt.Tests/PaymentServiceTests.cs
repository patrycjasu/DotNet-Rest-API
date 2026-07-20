using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Projekt.Data;
using Projekt.DTOs.OrderDtos;
using Projekt.DTOs.PaymentDtos;
using Projekt.Entities.ClientModels;
using Projekt.Entities.OrderModels;
using Projekt.Entities.ProductModels;
using Projekt.Enums;
using Projekt.Exceptions;
using Projekt.Migrations;
using Projekt.Services;
using Projekt.Tests.Helpers;
using System;

namespace Projekt.Tests
{
    public class PaymentServiceTests
    {

        [Fact]
        public async Task AddPaymentShouldAddPayment()
        {
            var logger = new Mock<ILogger<PaymentService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new PaymentService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context);

            var dto = new AddPaymentDto
            {
                Amount = new decimal(1),
                Method = Enums.Method.CARD
            };

            await service.AddPayment(1, dto, CancellationToken.None);

            var payment = await context.Payments.FirstOrDefaultAsync();

            Assert.NotNull(payment);
            Assert.Equal(new decimal(1), payment.Amount);
            Assert.Equal(1, payment.Id);
            Assert.Equal(Enums.Method.CARD, payment.Method);
            Assert.Equal(1, payment.OrderId);

        }

        [Fact]
        public async Task AddPaymentWithWrongOrderIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<PaymentService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new PaymentService(context, logger.Object);

            var dto = new AddPaymentDto
            {
                Amount = new decimal(1),
                Method = Enums.Method.CARD
            };
            await Assert.ThrowsAsync<NotFoundException>(() => service.AddPayment(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task AddPaymentToAlreadyPaidOrderShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<PaymentService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new PaymentService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context, state: State.PAID);

            var dto = new AddPaymentDto
            {
                Amount = new decimal(1),
                Method = Enums.Method.CARD
            };
            await Assert.ThrowsAsync<BadRequestException>(() => service.AddPayment(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task AddPaymentToCancelledOrderShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<PaymentService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new PaymentService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context, state: State.CANCELLED);

            var dto = new AddPaymentDto
            {
                Amount = new decimal(1),
                Method = Enums.Method.CARD
            };

            await Assert.ThrowsAsync<BadRequestException>(() => service.AddPayment(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task AddPaymentToDeletedOrderShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<PaymentService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new PaymentService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context, isDeleted: true);
            var dto = new AddPaymentDto
            {
                Amount = new decimal(1),
                Method = Enums.Method.CARD
            };

            await Assert.ThrowsAsync<BadRequestException>(() => service.AddPayment(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task AddPaymentWithNegativeAmountShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<PaymentService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new PaymentService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context);

            var dto = new AddPaymentDto
            {
                Amount = new decimal(-1),
                Method = Enums.Method.CARD
            };
            await Assert.ThrowsAsync<BadRequestException>(() => service.AddPayment(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task AddPaymentWithExceedingAmountShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<PaymentService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new PaymentService(context, logger.Object);

            await TestDataSeeder.SeedOrder(context);

            var dto = new AddPaymentDto
            {
                Amount = new decimal(10000000),
                Method = Enums.Method.CARD
            };
            await Assert.ThrowsAsync<BadRequestException>(() => service.AddPayment(1, dto, CancellationToken.None));
        }
    }
}
