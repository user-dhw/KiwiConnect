namespace InfoExchange.Api.Infrastructure.Security;

public interface IMd5Hasher
{
  string Hash(string value);
}
