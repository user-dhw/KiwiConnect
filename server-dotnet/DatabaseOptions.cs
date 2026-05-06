public sealed class DatabaseOptions
{
  public string Host { get; set; } = "localhost";
  public int Port { get; set; } = 3306;
  public string User { get; set; } = "root";
  public string Password { get; set; } = string.Empty;
  public string Database { get; set; } = "info-exchange";
}
