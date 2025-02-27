using Domain.Models.Projects;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IProjectQueries
{
    Task<IReadOnlyList<Project>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<Project>> GetAllByCreator(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Project>> GetAllByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Option<Project>> GetByName(string name, CancellationToken cancellationToken);
    Task<Option<Project>> GetById(ProjectId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Project>> GetAllByClient(Guid userId, CancellationToken cancellationToken);
    
    Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetAllPaginated(int page, int pageSize, CancellationToken cancellationToken);
    Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetAllByUserIdPaginated(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
}