using Domain.Models.Projects;

namespace Tests.Data;

public static class ProjectsData
{
    public static Project NewProject(Guid creatorId, Guid clientId) => 
        Project.New
    (
        ProjectId.New(), 
        "NewProject",
        "NewProjectDescription",
        DateTime.UtcNow,
        creatorId,
        "#000000",
        clientId
    );
}