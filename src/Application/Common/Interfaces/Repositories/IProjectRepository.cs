using Domain.Models.Projects;

namespace Application.Common.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<Project> Add(Project project, CancellationToken cancellationToken);
    Task<Project> Update(Project project, CancellationToken cancellationToken);
    Task<Project> Delete(Project project, CancellationToken cancellationToken);
}