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
    
    public static Project ExistingProject(Guid creatorId, Guid clientId) => 
        Project.New
        (
            ProjectId.New(), 
            "ExistingProject",
            "ExistingProjectDescription",
            DateTime.UtcNow,
            creatorId,
            "#000fff",
            clientId
        );
    
    public static Project ExistingProjectForProjectUser(Guid creatorId, Guid clientId) => 
        Project.New
        (
            ProjectId.New(), 
            "ExistingProjectForProjectUser",
            "ExistingProjectForProjectUserDescription",
            DateTime.UtcNow,
            creatorId,
            "#000fff",
            clientId
        );
    
    public static Project ExistingProject2(Guid creatorId, Guid clientId) => 
        Project.New
        (
            ProjectId.New(), 
            "ExistingProject2",
            "ExistingProjectDescription2",
            DateTime.UtcNow,
            creatorId,
            "#ffffff",
            clientId
        );
}