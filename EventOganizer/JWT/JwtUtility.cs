namespace EventOganizer.JWT
{
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class JwtUtility
    {
        public static bool ValidateToken(string jwtToken, out ClaimsPrincipal principal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("houseofthedragonkey")),
                ValidateIssuer = true,
                ValidIssuer = "houseofdragonhere",
                ValidateAudience = true,
                ValidAudience = "houseofdragonyour",
                ClockSkew = TimeSpan.Zero 
            };

            try
            {
                principal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out var validatedToken);
                return true;
            }
            catch
            {
                principal = null;
                return false;
            }
        }

        public static List<string> GetUserRoles(ClaimsPrincipal principal)
        {
            var userRoles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            return userRoles;
        }
    }

}
