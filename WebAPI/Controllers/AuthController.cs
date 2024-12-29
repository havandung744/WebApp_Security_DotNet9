using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "admin" && credential.Password == "password")
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "hdung72267@gmail.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                };
                var expiresAt = DateTime.Now.AddSeconds(10);
                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt
                });

            }
            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint.");
            return Unauthorized(ModelState);
        }
        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey") ?? "");
            var jwt = new JwtSecurityToken(
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: expireAt,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(secretKey),
                        SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class Credential
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
