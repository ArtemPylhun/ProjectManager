using Application.Common;
using Application.Roles.Exceptions;
using Domain.Models.Roles;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Commands;

public record DeleteRoleCommand : IRequest<Result<Role, RoleException>>
{
    public Guid Id { get; init; }
}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<Role, RoleException>>
{
    private readonly RoleManager<Role> _roleManager;

    public DeleteRoleCommandHandler(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<Role, RoleException>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = await _roleManager.FindByIdAsync(request.Id.ToString());
        if (existingRole == null)
        {
            return await Task.FromResult(Result<Role, RoleException>.Failure(new RoleNotFoundException(Guid.Empty)));
        }

        var result = _roleManager.DeleteAsync(existingRole).Result;
        return await Task.FromResult(Result<Role, RoleException>.FromIdentityResult<Role, RoleException>(result, existingRole,
            e => new RoleUnknownException(existingRole.Id, new Exception(e.ToString()))));
    }
}