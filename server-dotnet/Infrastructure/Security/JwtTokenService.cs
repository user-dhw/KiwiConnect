using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InfoExchange.Api.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
  private readonly JwtOptions _options;

  public JwtTokenService(IOptions<JwtOptions> options)
  {
    _options = options.Value;
  }

  public string CreateUserToken(string uid, string nickname)
  {
    var claims = new List<Claim>
        {
            new("uid", uid),
            new("nickname", nickname)
        };

    return BuildToken(claims);
  }

  public string CreateAdminToken(string uid, string username, object jurisdiction)
  {
    var claims = new List<Claim>
        {
            new("uid", uid),
            new("username", username)
        };

    foreach (var prop in jurisdiction.GetType().GetProperties())
    {
      var value = prop.GetValue(jurisdiction)?.ToString() ?? string.Empty;
      claims.Add(new Claim(prop.Name, value));
    }

    return BuildToken(claims);
  }

  private string BuildToken(IEnumerable<Claim> claims)
  {
    var key = JwtKeyHelper.CreateSigningKey(_options.PrivateKey);
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: _options.Issuer,
        audience: _options.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddSeconds(_options.ExpireSeconds),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
