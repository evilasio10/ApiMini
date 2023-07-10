using ApiMini.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiMini.Service
{
    public static class TokenService
    {
        public static string GerarToken(Usuario usuario, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName),
                    new Claim(ClaimTypes.Role, usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDecriptor);
            return tokenHandler.WriteToken(token);
        }

        public static String SHA256(string input)
        {
            return SHA256(UTF8Encoding.UTF8.GetBytes(input));
        }

        private static string SHA256(Byte[] input)
        {
            using (System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256.Create())
            {
                return BitConverter.ToString(hash.ComputeHash(input)).Replace("-", "").ToLower();
            }
        }

    }
}

