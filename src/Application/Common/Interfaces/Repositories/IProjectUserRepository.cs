using Domain.Models.ProjectUsers;

namespace Application.Common.Interfaces.Repositories;

public interface IProjectUserRepository
{
    Task<ProjectUser> Add(ProjectUser projectUser, CancellationToken cancellationToken);
    Task<ProjectUser> Update(ProjectUser projectUser, CancellationToken cancellationToken);
    Task<ProjectUser> Delete(ProjectUser projectUser, CancellationToken cancellationToken);
}