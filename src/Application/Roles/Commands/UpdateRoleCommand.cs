using Application.Common;
using Application.Roles.Exceptions;
using Domain.Models.Roles;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Commands;

public record UpdateRoleCommand : IRequest<Result<Role, RoleException>>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<Role, RoleException>>
{
    private readonly RoleManager<Role> _roleManager;

    public UpdateRoleCommandHandler(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public Task<Result<Role, RoleException>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = _roleManager.FindByIdAsync(request.Id.ToString()).Result;
        if (existingRole == null)
        {
            return Task.FromResult(Result<Role, RoleException>.Failure(new RoleNotFoundException(Guid.Empty)));
        }
        
        var existingRoleName = _roleManager.FindByNameAsync(request.Name).Result;
        if (existingRoleName != null && existingRoleName.Id != existingRole.Id)
        {
            return Task.FromResult(Result<Role, RoleException>.Failure(new RoleAlreadyExistsException(existingRole.Id)));
        }

        existingRole.Name = request.Name;
        var result = _roleManager.UpdateAsync(existingRole).Result;
        return Task.FromResult(Result<Role, RoleException>.FromIdentityResult<Role, RoleException>(result, existingRole,
            e => new RoleUnknownException(existingRole.Id, new Exception(e.ToString()))));
    }
}