using Domain.Models.Projects;
using Domain.Models.TimeEntries;
using Domain.Models.Users;

namespace Domain.Models.Tasks;

public class Task
{
    public TaskId Id { get; }
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

}