using Domain.Models.Roles;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IRoleQueries
{
    Task<IReadOnlyList<Role>> GetAll(CancellationToken cancellationToken);
    Task<Option<Role>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Option<Role>> GetById(Guid id, CancellationToken cancellationToken);
}