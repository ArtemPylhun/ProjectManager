using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("users")]
[ApiController]
public class UsersController(ISender sender, IUserQueries userQueries) : ControllerBase
{
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}