using System.Data;

namespace InfoExchange.Api.Infrastructure.Data;

public interface IDbConnectionFactory
{
  IDbConnection CreateConnection();
}
