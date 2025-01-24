using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;
using Domain.Models.UsersTasks;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface ITimeEntryQueries
{
    Task<IReadOnlyList<TimeEntry>> GetAll(CancellationToken cancellationToken);

    Task<IReadOnlyList<TimeEntry>> GetAllByProjectTaskId(ProjectTaskId projectTaskId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TimeEntry>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TimeEntry>> GetAllByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Option<TimeEntry>> GetById(TimeEntryId id, CancellationToken cancellationToken);

    Task<Option<TimeEntry>> GetByProjectTaskAndProjectAndUserIds(ProjectTaskId projectTaskId, ProjectId projectId,
        Guid userId, CancellationToken cancellationToken);
}