using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] UserLogin userLogin)
    {
        if (userLogin.Username == "Admin" && userLogin.Password == "Password")
        {
            var token = GenerateJwtToken(userLogin.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }

    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyThatIsLongEnoughForHS256"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(ClaimTypes.Role, "Admin") // Aquí agregamos el rol de administrador
        };

        var token = new JwtSecurityToken(
            issuer: "yourdomain.com",
            audience: "yourdomain.com",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}