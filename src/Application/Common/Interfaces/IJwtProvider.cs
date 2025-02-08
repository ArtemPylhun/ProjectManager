using Domain.Models.Roles;
using Domain.Models.Users;

namespace Application.Common.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user, List<string> roles);
}