using Infrastructure;

namespace Service.Authorization;

public interface IAuthorizationService
{
    public string GenerateToken(User user);
}