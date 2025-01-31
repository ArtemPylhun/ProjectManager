    using Domain.Models.Projects;

namespace API.DTOs;

public record ProjectDto(
    Guid Id,
    string Name,
    string Description,
    string ColorHex,
    DateTime CreatedAt,
    Guid CreatorId,
    Guid ClientId)
{
    public static ProjectDto FromDomainModel(Project project)
     => new(
         Id: project.Id.Value,
         Name: project.Name,
         Description: project.Description,
         ColorHex: project.ColorHex,
         CreatedAt: project.CreatedAt,
         CreatorId: project.CreatorId,
         ClientId: project.ClientId);
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