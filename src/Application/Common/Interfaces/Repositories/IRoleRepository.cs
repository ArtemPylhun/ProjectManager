using Domain.Models.Roles;

namespace Application.Common.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role> Add(Role role, CancellationToken cancellationToken);
    Task<Role> Update(Role role, CancellationToken cancellationToken);
    Task<Role> Delete(Role role, CancellationToken cancellationToken);
}