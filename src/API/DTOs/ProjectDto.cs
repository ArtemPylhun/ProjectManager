    using Domain.Models.Projects;
    using Domain.Models.Users;

    namespace API.DTOs;

public record ProjectDto(
    Guid Id,
    string Name,
    string Description,
    string ColorHex,
    DateTime CreatedAt,
    Guid CreatorId,
    Guid ClientId,
    User? Creator,
    User? Client)
{
    public static ProjectDto FromDomainModel(Project project)
     => new(
         Id: project.Id.Value,
         Name: project.Name,
         Description: project.Description,
         ColorHex: project.ColorHex,
         CreatedAt: project.CreatedAt,
         CreatorId: project.CreatorId,
         ClientId: project.ClientId,
         Creator: project.Creator,
         Client: project.Client);
}

public record ProjectCreateDto(
    string Name,
    string Description,
    string ColorHex,
    DateTime CreatedAt,
    Guid CreatorId,
    Guid ClientId)
{
   
}

public record ProjectUpdateDto(
    Guid Id,
    string Name,
    string Description,
    string ColorHex,
    Guid ClientId)
{
   
}