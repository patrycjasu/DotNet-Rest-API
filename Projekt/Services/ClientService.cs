using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.DTOs.ClientDtos;
using Projekt.Entities.ClientModels;
using Projekt.Exceptions;
using Projekt.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace Projekt.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ClientService> _logger;

        public ClientService(AppDbContext dbContext, ILogger<ClientService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddClientCompany(AddClientCompanyDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating company client");
            var nipExists = await _dbContext.Companies.AnyAsync(x => x.NIP == dto.NIP, cancellationToken);
            if (nipExists)
            {
                _logger.LogWarning("Cannot create company");
                throw new BadRequestException("Klient z tym NIPEM juz istnieje");
            }

            var client = new Company
            {
                Name = dto.Name!,
                NIP = dto.NIP!,
                Phone = dto.Phone!,
                Email = dto.Email!,
                Address = dto.Address!
            };

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Company client created with id {client.Id}", client.Id);
        }

        public async Task AddClientPerson(AddClientPersonDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating private client");
            var peselExists = await _dbContext.PrivateClients.AnyAsync(x => x.PESEL == dto.PESEL, cancellationToken);
            if (peselExists)
            {
                _logger.LogWarning("Cannot create private person");
                throw new BadRequestException("Klient z tym PESELEM juz istnieje");
            }

            var client = new PrivatePerson
            {
                FirstName = dto.FirstName!,
                LastName = dto.LastName!,
                PESEL = dto.PESEL!,
                Phone = dto.Phone!,
                Email = dto.Email!,
                Address = dto.Address!
            };

            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Private client created with id {client.Id}", client.Id);
        }

        public async Task<GetClientDto> GetClientById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting client with {id}", id);
            var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (client == null){
                _logger.LogWarning($"Cannot find client");
                throw new NotFoundException("Nie ma klienta z takim id");
            }
            var res = new GetClientDto
            {
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address

            };
            if (client is PrivatePerson person)
            {
                res.FirstName = person.FirstName;
                res.LastName = person.LastName;
                res.PESEL = person.PESEL;

            } else if (client is Company company)
            {
                res.Name = company.Name;
                res.NIP = company.NIP;
            }
            res.Orders = await _dbContext.Orders.Include(x=> x.OrderProducts).ThenInclude(x=> x.Product).ThenInclude(x=>x.Category)
                .Where(x => x.ClientId == id)
                .Select(o => new GetOrderForClientDto(){
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    TotalWeight = o.TotalWeight,
                    GetProductsForOrdersForClient = o.OrderProducts
                    .Select(a => new GetProductForOrdersForClientDto()
                    {
                        Name = a.Product.Name,
                        Description = a.Product.Description,
                        CategoryName = a.Product.Category.Name,
                        Quantity = a.Quantity,
                        UnitPrice = a.UnitPrice
                    }).ToList()
                }).ToListAsync(cancellationToken);
            return res;
        }

        public async Task<IEnumerable<GetClientCompanyDto>> GetClientsCompanies(string? name, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting client");
            var clients = await _dbContext.Companies.Include(x=> x.Orders).ThenInclude(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
                .Where(x => name == null || x.Name.ToLower().Contains(name.ToLower()))
                .Select(x => new GetClientCompanyDto()
                {
                    Id = x.Id,
                    Email = x.Email,
                    Phone = x.Phone,
                    Name = x.Name,
                    Address = x.Address,
                    NIP = x.NIP,
                    Orders = x.Orders.Select(a => new GetOrderForClientDto
                    {
                        Id = a.Id,
                        TotalPrice = a.TotalPrice,
                        TotalWeight = a.TotalWeight,
                        GetProductsForOrdersForClient = a.OrderProducts.Select(o=> new GetProductForOrdersForClientDto()
                        {
                            Name = o.Product.Name,
                            Description= o.Product.Description,
                            CategoryName= o.Product.Category.Name,
                            Quantity = o.Quantity,
                            UnitPrice = o.UnitPrice
                        })
                    })
                })
                .ToListAsync(cancellationToken);

            return clients;
        }

        public async Task<IEnumerable<GetClientPersonDto>> GetClientsPeople(string? firstName, string? lastName, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting client");

            var clients = await _dbContext.PrivateClients.Include(x => x.Orders).ThenInclude(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
                .Where(x => 
                (firstName == null & lastName == null)|| 
                (firstName != null && x.FirstName.ToLower().Contains(firstName.ToLower())) ||
                (lastName != null && x.LastName.ToLower().Contains(lastName.ToLower())))
                .Select(x => new GetClientPersonDto()
                {
                    Id = x.Id,
                    Email = x.Email,
                    Phone = x.Phone,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address = x.Address,
                    PESEL = x.PESEL,
                    Orders = x.Orders.Select(a => new GetOrderForClientDto
                    {
                        Id = a.Id,
                        TotalPrice = a.TotalPrice,
                        TotalWeight = a.TotalWeight,
                        GetProductsForOrdersForClient = a.OrderProducts.Select(o => new GetProductForOrdersForClientDto()
                        {
                            Name = o.Product.Name,
                            Description = o.Product.Description,
                            CategoryName = o.Product.Category.Name,
                            Quantity = o.Quantity,
                            UnitPrice = o.UnitPrice
                        })
                    })
                })
                .ToListAsync(cancellationToken);

            return clients;
        }

        public async Task UpdateClientCompany(int id, UpdateClientCompanyDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating client with {id}", id);

            var client = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (client == null)
            {
                _logger.LogWarning("Cannot find client");
                throw new NotFoundException("Nie ma takiej firmy");
            }
            client.Phone = dto.Phone!;
            client.Email = dto.Email!;
            client.Address = dto.Address!;
            client.Name = dto.Name!;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateClientPerson(int id, UpdateClientPersonDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating client with {id}", id);
            var client = await _dbContext.PrivateClients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (client == null)
            {
                _logger.LogWarning("Cannot find client");
                throw new NotFoundException("Nie ma takiej osoby");
            }
            client.Phone = dto.Phone!;
            client.Email = dto.Email!;
            client.Address = dto.Address!;
            client.FirstName = dto.FirstName!;
            client.LastName = dto.LastName!;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
