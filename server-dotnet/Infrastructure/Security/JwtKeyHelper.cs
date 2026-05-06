using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace InfoExchange.Api.Infrastructure.Security;

public static class JwtKeyHelper
{
  public static SymmetricSecurityKey CreateSigningKey(string secret)
  {
    var raw = Encoding.UTF8.GetBytes(secret ?? string.Empty);
    if (raw.Length >= 16)
    {
      return new SymmetricSecurityKey(raw);
    }

    var expanded = SHA256.HashData(raw);
    return new SymmetricSecurityKey(expanded);
  }
}
