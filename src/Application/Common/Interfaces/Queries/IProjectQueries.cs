using Domain.Models.Projects;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IProjectQueries
{
    Task<IReadOnlyList<Project>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<Project>> GetAllByCreator(Guid userId, CancellationToken cancellationToken);
    Task<Option<Project>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Option<Project>> GetById(ProjectId id, CancellationToken cancellationToken);
    
}