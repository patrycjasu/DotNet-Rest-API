using Projekt.DTOs.ClientDtos;
using Projekt.DTOs.ProductDtos;

namespace Projekt.Services.Interfaces
{
    public interface IClientService
    {
        Task<GetClientDto> GetClientById(int id, CancellationToken cancellationToken);
        Task <IEnumerable<GetClientPersonDto>> GetClientsPeople(string? firstName, string? lastName, CancellationToken cancellationToken);
        Task AddClientPerson(AddClientPersonDto dto, CancellationToken cancellationToken);
        Task UpdateClientPerson(int id, UpdateClientPersonDto dto, CancellationToken cancellationToken);
        Task<IEnumerable<GetClientCompanyDto>> GetClientsCompanies(string? name, CancellationToken cancellationToken);
        Task AddClientCompany(AddClientCompanyDto dto, CancellationToken cancellationToken);
        Task UpdateClientCompany(int id, UpdateClientCompanyDto dto, CancellationToken cancellationToken);
    }
}
