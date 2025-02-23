using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IProjectTaskQueries
{
    Task<IReadOnlyList<ProjectTask>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectTask>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken);
    Task<Option<ProjectTask>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Option<ProjectTask>> GetById(ProjectTaskId id, CancellationToken cancellationToken);
    
}