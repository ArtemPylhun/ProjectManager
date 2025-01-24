using Domain.Models.Projects;
using Domain.Models.ProjectUsers;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IProjectUserQueries
{
    Task<IReadOnlyList<ProjectUser>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectUser>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectUser>> GetAllByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Option<ProjectUser>> GetById(ProjectUserId id, CancellationToken cancellationToken);
    Task<Option<ProjectUser>> GetByProjectAndUserIds(ProjectId projectId, Guid userId, CancellationToken cancellationToken);
}