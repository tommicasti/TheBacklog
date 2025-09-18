using GameShelf.API.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using GameShelf.API.Backend.DTOs;
using GameShelf.API.Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using GameShelf.API.Services;



[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserService _userService;
    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _config = configuration;
        _userService = userService; 
    }


    [HttpGet("ciao")]
    public string Ciao()
    {
        return "ciao";
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (await _userService.DoesUserExistAsync(registerDto.Username))
        {
            return BadRequest("Username già esistente");
        }

        await _userService.RegisterUserAsync(registerDto);

        return Ok(new { message = "registrazione effettuata" });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var user = await _userService.GetUserByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))

        {
            return Unauthorized("Credenziali errate");
        }
        var token = GenerateJwtToken(user);
        return  Ok(new AuthResponseDto(token));


    }
}
