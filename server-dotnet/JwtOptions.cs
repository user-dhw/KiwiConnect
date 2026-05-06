public sealed class JwtOptions
{
  public string PrivateKey { get; set; } = "asdadadada";
  public int ExpireSeconds { get; set; } = 3600;
  public string Issuer { get; set; } = "info-exchange";
  public string Audience { get; set; } = "info-exchange-client";
}
