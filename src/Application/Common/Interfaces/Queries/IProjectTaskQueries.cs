using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IProjectTaskQueries
{
    Task<IReadOnlyList<ProjectTask>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectTask>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectTask>> GetAllByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Option<ProjectTask>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Option<ProjectTask>> GetById(ProjectTaskId id, CancellationToken cancellationToken);
    
    Task<(IReadOnlyList<ProjectTask> ProjectTasks, int TotalCount)> GetAllPaginated(int page, int pageSize, string search, CancellationToken cancellationToken);
    Task<(IReadOnlyList<ProjectTask> ProjectTasks, int TotalCount)> GetAllByUserIdPaginated(Guid userId, int page, int pageSize, string search, CancellationToken cancellationToken);
    

}