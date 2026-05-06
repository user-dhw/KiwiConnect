using System.Security.Cryptography;
using System.Text;

namespace InfoExchange.Api.Infrastructure.Security;

public sealed class Md5Hasher : IMd5Hasher
{
  public string Hash(string value)
  {
    var bytes = Encoding.UTF8.GetBytes(value);
    var hash = MD5.HashData(bytes);
    var sb = new StringBuilder(hash.Length * 2);

    foreach (var b in hash)
    {
      sb.Append(b.ToString("x2"));
    }

    return sb.ToString();
  }
}
