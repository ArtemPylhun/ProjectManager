using Domain.Models.Projects;
using Domain.Models.ProjectTasks;

namespace Application.Common.Interfaces.Repositories;

public interface IProjectTaskRepository
{
    Task<ProjectTask> Add(ProjectTask projectTask, CancellationToken cancellationToken);
    Task<ProjectTask> Update(ProjectTask projectTask, CancellationToken cancellationToken);
    Task<ProjectTask> Delete(ProjectTask projectTask, CancellationToken cancellationToken);
}