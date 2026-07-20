using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Projekt.Data;
using Projekt.DTOs.ProductDtos;
using Projekt.Entities.ProductModels;
using Projekt.Exceptions;
using Projekt.Services;
using Projekt.Tests.Helpers;
using System;

namespace Projekt.Tests
{
    public class ProductServiceTests
    {

        [Fact]
        public async Task AddProductShouldAddProduct()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            await TestDataSeeder.SeedCategory(context);

            var dto = new AddProductDto
            {
                Name = "A",
                Weight = new decimal(2),
                CurrentPrice = new decimal(2),
                CategoryId = 1,
            };

            var service = new ProductService(context, logger.Object);

            await service.AddProduct(dto, CancellationToken.None);
            var product = await context.Products.FirstOrDefaultAsync();
            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
            Assert.Equal("A", product.Name);
            Assert.Null(product.Description);
            Assert.Equal(new decimal(2), product.Weight);
            Assert.Equal(new decimal(2), product.CurrentPrice);
            Assert.Equal(1, product.CategoryId);
            Assert.False(product.IsDeleted);
        }

        [Fact]
        public async Task AddProductShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var dto = new AddProductDto
            {
                Name = "A",
                Weight = new decimal(2),
                CurrentPrice = new decimal(2),
                CategoryId = 1,
            };

            var service = new ProductService(context, logger.Object);
            await Assert.ThrowsAsync<NotFoundException>(() => service.AddProduct(dto, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteProductShouldDeleteProduct()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);

            await TestDataSeeder.SeedProduct(context);

            await service.DeleteProduct(1, CancellationToken.None);

            var product = await context.Products.FirstOrDefaultAsync();
            Assert.NotNull(product);
            Assert.True(product.IsDeleted);
        }

        [Fact]
        public async Task DeleteProductWithNotExistingIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteProduct(1, CancellationToken.None));

        }

        [Fact]
        public async Task DeleteProductAlreadyDeletedShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);

            await TestDataSeeder.SeedProduct(context, isDeleted:true);

            await Assert.ThrowsAsync<BadRequestException>(() => service.DeleteProduct(1, CancellationToken.None));

        }

        [Fact]
        public async Task GetAllProductsShouldReturnListOfAllProducts()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);

            await TestDataSeeder.SeedProduct(context, id:1, isDeleted:false, name:"A");
            await TestDataSeeder.SeedProduct(context, id: 2, isDeleted: true, name: "B");
            await TestDataSeeder.SeedProduct(context, id: 3, isDeleted: false, name: "A");

            var result = await service.GetAllProducts(null, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }
        [Fact]
        public async Task GetAllProductsByNameShouldReturnListOfProductsByName()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);


            await TestDataSeeder.SeedProduct(context, id: 1, isDeleted: false, name: "A");
            await TestDataSeeder.SeedProduct(context, id: 2, isDeleted: true, name: "B");
            await TestDataSeeder.SeedProduct(context, id: 3, isDeleted: false, name: "A");

            var result = await service.GetAllProducts("A", CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
        [Fact]
        public async Task GetAllProductsShouldReturnEmptyList()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);


            await TestDataSeeder.SeedProduct(context, id: 1, isDeleted: false, name: "A");
            await TestDataSeeder.SeedProduct(context, id: 2, isDeleted: true, name: "B");
            await TestDataSeeder.SeedProduct(context, id: 3, isDeleted: false, name: "A");

            var result = await service.GetAllProducts("C", CancellationToken.None);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductByIdShouldReturnProductDto()
        {

            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);


            await TestDataSeeder.SeedProduct(context);

            var res = await service.GetProductById(1, CancellationToken.None);
            Assert.NotNull(res);
            Assert.Equal("A", res.Name);
            Assert.Null(res.Description);
            Assert.Equal(new decimal(2), res.Weight);
            Assert.Equal(new decimal(2), res.CurrentPrice);
            Assert.Equal("Category", res.CategoryName);
            Assert.Null(res.CategoryDescription);

        }


        [Fact]
        public async Task GetProductByIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetProductById(1, CancellationToken.None));
        }

        [Fact]
        public async Task GetProductsByCategoryShouldReturnListOfProducs()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);


            await TestDataSeeder.SeedProduct(context, id: 1, isDeleted: false, name: "A");
            await TestDataSeeder.SeedProduct(context, id: 2, isDeleted: true, name: "B");
            await TestDataSeeder.SeedProduct(context, id: 3, isDeleted: false, name: "A");

            var result = await service.GetProductsByCategory("Category", CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());

        }

        [Fact]
        public async Task GetProductsByCategoryShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetProductsByCategory("category", CancellationToken.None));
        }

        [Fact]
        public async Task UpdateProductShouldUpdateProduct()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);

            await TestDataSeeder.SeedProduct(context);

            var dto = new UpdateProductDto
            {
                Description = "B",
                CurrentPrice = new decimal(5),
            };

            await service.UpdateProduct(dto, 1, CancellationToken.None);
            var res = await context.Products.FirstOrDefaultAsync();
            Assert.NotNull(res);
            Assert.Equal("A", res.Name);
            Assert.Equal("B", res.Description);
            Assert.Equal(new decimal(2), res.Weight);
            Assert.Equal(new decimal(5), res.CurrentPrice);
            Assert.Equal("Category", res.Category.Name);
            Assert.Null(res.Category.Description);
        }

        [Fact]
        public async Task UpdateProductWithNotExistingIdShouldThrowNotFoundException()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);
            var dto = new UpdateProductDto
            {
                Description = "B",
                CurrentPrice = new decimal(5),
            };
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateProduct(dto, 1, CancellationToken.None));

        }

        [Fact]
        public async Task UpdateProductAlreadyDeletedShouldThrowBadRequestException()
        {
            var logger = new Mock<ILogger<ProductService>>();
            var context = DbContextFactory.GetDbContext();
            var service = new ProductService(context, logger.Object);

            await TestDataSeeder.SeedProduct(context,isDeleted: true);

            var dto = new UpdateProductDto
            {
                Description = "B",
                CurrentPrice = new decimal(5),
            };

            await Assert.ThrowsAsync<BadRequestException>(() => service.UpdateProduct(dto, 1, CancellationToken.None));



        }

    }
}
