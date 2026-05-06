namespace InfoExchange.Api.Infrastructure.Security;

public interface IJwtTokenService
{
  string CreateUserToken(string uid, string nickname);
  string CreateAdminToken(string uid, string username, object jurisdiction);
}
