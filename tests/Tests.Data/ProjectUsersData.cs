using Domain.Models.Projects;
using Domain.Models.ProjectUsers;

namespace Tests.Data;

public static class ProjectUsersData
{
    public static ProjectUser NewProjectUser(ProjectId projectId, Guid userId, Guid roleId) => 
        ProjectUser.New(ProjectUserId.New(), projectId, userId, roleId);
    
    public static ProjectUser ExistingProjectUser(ProjectId projectId, Guid userId, Guid roleId) => 
        ProjectUser.New(ProjectUserId.New(), projectId, userId, roleId);
}