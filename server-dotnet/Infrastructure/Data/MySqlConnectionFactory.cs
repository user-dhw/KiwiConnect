using System.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace InfoExchange.Api.Infrastructure.Data;

public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
  private readonly DatabaseOptions _options;

  public MySqlConnectionFactory(IOptions<DatabaseOptions> options)
  {
    _options = options.Value;
  }

  public IDbConnection CreateConnection()
  {
    var builder = new MySqlConnectionStringBuilder
    {
      Server = _options.Host,
      Port = (uint)_options.Port,
      UserID = _options.User,
      Password = _options.Password,
      Database = _options.Database,
      CharacterSet = "utf8mb4",
      AllowUserVariables = true
    };

    return new MySqlConnection(builder.ConnectionString);
  }
}
