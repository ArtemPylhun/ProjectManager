using Application.Common;
using Application.Roles.Exceptions;
using Domain.Models.Roles;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Commands;

public record CreateRoleCommand: IRequest<Result<Role, RoleException>>
{
    public string Name { get; init; }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Role, RoleException>>
{
    private readonly RoleManager<Role> _roleManager;

    public CreateRoleCommandHandler(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public Task<Result<Role, RoleException>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = _roleManager.FindByNameAsync(request.Name).Result;
        if (existingRole != null)
        {
            return Task.FromResult(Result<Role, RoleException>.Failure(new RoleAlreadyExistsException(existingRole.Id)));
        }
        
        var role = new Role { Name = request.Name };
        var result = _roleManager.CreateAsync(role).Result;
        return Task.FromResult(Result<Role, RoleException>.FromIdentityResult<Role, RoleException>(result, role, e => new RoleUnknownException(role.Id, new Exception(e.ToString()))));
    }
}
