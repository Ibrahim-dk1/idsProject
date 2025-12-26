using System.Security.Cryptography;
using System.Text;

namespace idsProject.Services
{
    public static class RefreshTokenHasher
    {
        public static string Hash(string token, string pepper)
        {
            // HMACSHA256(token, pepper) -> stable hash string
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(pepper));
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
