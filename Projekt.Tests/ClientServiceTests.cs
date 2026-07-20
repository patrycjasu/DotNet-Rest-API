using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.DTOs.ClientDtos;
using Projekt.Services;
using System;
using Moq;
using Microsoft.Extensions.Logging;
using Projekt.Entities.ClientModels;
using Projekt.Exceptions;
using Projekt.Tests.Helpers;

public class ClientServiceTests
{


	[Fact]
	public async Task AddClientPersonShouldAddPerson()
	{
		var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();
        var service = new ClientService(context, logger.Object);
		var dto = new AddClientPersonDto
		{
			FirstName = "Jane",
			LastName = "Doe",
			Email = "janedoe@mail.com",
			Phone = "123456789",
			Address = "example address",
			PESEL = "12345678901"
		};
		await service.AddClientPerson(dto, CancellationToken.None);
		var client = await context.Clients.OfType<PrivatePerson>().FirstOrDefaultAsync();
		Assert.NotNull(client);
		Assert.Equal("Jane", client.FirstName);
		Assert.Equal("Doe", client.LastName);
		Assert.Equal("janedoe@mail.com", client.Email);
		Assert.Equal("123456789", client.Phone);
		Assert.Equal("example address", client.Address);
		Assert.Equal("12345678901", client.PESEL);
	}

    [Fact]
    public async Task AddClientCompanyShouldAddCompany()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();
        var service = new ClientService(context, logger.Object);
        var dto = new AddClientCompanyDto
        {
            Name = "Company",
            Email = "company@mail.com",
            Phone = "123456789",
            Address = "example address",
            NIP = "1234567890"
        };
        await service.AddClientCompany(dto, CancellationToken.None);
        var client = await context.Clients.OfType<Company>().FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal("Company", client.Name);;
        Assert.Equal("company@mail.com", client.Email);
        Assert.Equal("123456789", client.Phone);
        Assert.Equal("example address", client.Address);
        Assert.Equal("1234567890", client.NIP);
    }

    [Fact]
    public async Task AddClientPersonWithAlreadyUsedPESELShouldThrowBadRequest()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context);

        var service = new ClientService(context, logger.Object);

        var dto = new AddClientPersonDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "janedoe@mail.com",
            Phone = "123456789",
            Address = "example address",
            PESEL = "12345678901"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => service.AddClientPerson(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddClientCompanyWithAlreadyUsedNIPShouldThrowBadRequest()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClientCompany(context);

        await context.SaveChangesAsync();
        var service = new ClientService(context, logger.Object);
        var dto = new AddClientCompanyDto
        {
            Name = "Company",
            Email = "company@mail.com",
            Phone = "123456789",
            Address = "example address",
            NIP = "1234567890"
        };
        await Assert.ThrowsAsync<BadRequestException>(() => service.AddClientCompany(dto, CancellationToken.None));
    }

    [Fact]
    public async Task GetPersonByIdShouldReturnClientDto()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context);

        await context.SaveChangesAsync();

        var service = new ClientService(context, logger.Object);
        var result = await service.GetClientById(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("johndoe@mail.com", result.Email);
        Assert.Equal("987654321", result.Phone);
        Assert.Equal("example address", result.Address);
        Assert.Equal("12345678901", result.PESEL);
    }

    [Fact]
    public async Task GetClientByIdShouldThrowNotFoundException()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();
        var service = new ClientService(context, logger.Object);
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetClientById(1, CancellationToken.None));
    }

    [Fact]
    public async Task GetCompanyByIdShouldReturnClientDto()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClientCompany(context);

        var service = new ClientService(context, logger.Object);
        var result = await service.GetClientById(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Company", result.Name);
        Assert.Equal("company@mail.com", result.Email);
        Assert.Equal("987654321", result.Phone);
        Assert.Equal("example address", result.Address);
        Assert.Equal("1234567890", result.NIP);
    }

    [Fact]
    public async Task GetClientCompaniesByNameShouldReturnAllCompanies()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClientCompany(context,1, "CompanyA", "1234567890");
        await TestDataSeeder.SeedClientCompany(context, 2, "CompanyB", "0987654321");


        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsCompanies(null, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetClientCompaniesByNameShouldReturnOneCompany()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClientCompany(context, 1, "CompanyA", "1234567890");
        await TestDataSeeder.SeedClientCompany(context, 2, "CompanyB", "0987654321");

        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsCompanies("CompanyA", CancellationToken.None);
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetClientCompaniesByNameShouldReturnEmptyList()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClientCompany(context, 1, "CompanyA", "1234567890");
        await TestDataSeeder.SeedClientCompany(context, 2, "CompanyB", "0987654321");

        await context.SaveChangesAsync();
        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsCompanies("CompanyC", CancellationToken.None);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetClientPeopleByFirstNameShouldReturnAllPeople()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context, 1, "John", "Doe", "12345678901");
        await TestDataSeeder.SeedClient(context, 2, "Jane", "Black", "09876543210");

        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsPeople(null, null, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetClientPeopleByFirstNameShouldReturnOnePerson()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context, 1, "John", "Doe", "12345678901");
        await TestDataSeeder.SeedClient(context, 2, "Jane", "Black", "09876543210");

        await context.SaveChangesAsync();
        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsPeople("John", null, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetClientPeopleByFistNameShouldReturnEmptyList()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context, 1, "John", "Doe", "12345678901");
        await TestDataSeeder.SeedClient(context, 2, "Jane", "Black", "09876543210");

        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsPeople("Janette", null, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    [Fact]
    public async Task GetClientPeopleByLastNameShouldReturnAllPeople()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context, 1, "John", "Doe", "12345678901");
        await TestDataSeeder.SeedClient(context, 2, "Jane", "Black", "09876543210");

        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsPeople(null, null, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetClientPeopleByLastNameShouldReturnOnePerson()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context, 1, "John", "Doe", "12345678901");
        await TestDataSeeder.SeedClient(context, 2, "Jane", "Black", "09876543210");
        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsPeople(null, "Doe", CancellationToken.None);
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetClientPeopleByLastNameShouldReturnEmptyList()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();


        await TestDataSeeder.SeedClient(context, 1, "John", "Doe", "12345678901");
        await TestDataSeeder.SeedClient(context, 2, "Jane", "Black", "09876543210");
        var service = new ClientService(context, logger.Object);

        var result = await service.GetClientsPeople(null, "Surname", CancellationToken.None);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateCompanyShouldUpdateCompany()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClientCompany(context);

        var service = new ClientService(context, logger.Object);

        var dto = new UpdateClientCompanyDto
        {
            Name = "CompanyB",
            Email = "companyb@mail.com",
            Phone = "987654321",
            Address = "example address2",
        };

        await service.UpdateClientCompany(1, dto, CancellationToken.None);

        var client = await context.Clients.OfType<Company>().FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal("CompanyB", client.Name);
        Assert.Equal("companyb@mail.com", client.Email);
        Assert.Equal("987654321", client.Phone);
        Assert.Equal("example address2", client.Address);
    }

    [Fact]
    public async Task UpdateCompanyShouldThrowNotFoundException()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();
        await context.SaveChangesAsync();
        var service = new ClientService(context, logger.Object);

        var dto = new UpdateClientCompanyDto
        {
            Name = "CompanyB",
            Email = "companyb@mail.com",
            Phone = "987654321",
            Address = "example address2",
        };
        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateClientCompany(999, dto, CancellationToken.None));

    }

    [Fact]
    public async Task UpdatePersonShouldUpdatePerson()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();

        await TestDataSeeder.SeedClient(context);

        var service = new ClientService(context, logger.Object);
        var dto = new UpdateClientPersonDto
        {
            FirstName = "Jane",
            LastName = "Black",
            Email = "janedoe@mail.com",
            Phone = "123456789",
            Address = "example address2"
        };
        await service.UpdateClientPerson(1, dto, CancellationToken.None);
        var client = await context.Clients.OfType<PrivatePerson>().FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal("Jane", client.FirstName);
        Assert.Equal("Black", client.LastName);
        Assert.Equal("janedoe@mail.com", client.Email);
        Assert.Equal("123456789", client.Phone);
        Assert.Equal("example address2", client.Address);

    }

    [Fact]
    public async Task UpdatePersonShouldThrowNotFoundException()
    {
        var logger = new Mock<ILogger<ClientService>>();
        var context = DbContextFactory.GetDbContext();
        var service = new ClientService(context, logger.Object);

        var dto = new UpdateClientPersonDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "janedoe@mail.com",
            Phone = "123456789",
            Address = "example address2"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateClientPerson(999, dto, CancellationToken.None));
    }

}
