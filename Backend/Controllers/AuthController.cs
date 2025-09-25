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

    /// <summary>
    /// Handles HTTP GET requests to the "ciao" endpoint and returns a greeting message.
    /// </summary>
    /// <returns>A string containing the greeting message "ciao".</returns>
    /// TEST USE
    [HttpGet("ciao")]
    public string Ciao()
    {
        return "ciao";
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user.
    /// </summary>
    /// <remarks>The generated token includes the user's ID as the subject claim (<see
    /// cref="JwtRegisteredClaimNames.Sub"/>) and a unique identifier as the token ID claim (<see
    /// cref="JwtRegisteredClaimNames.Jti"/>). The token is signed using the HMAC-SHA256 algorithm and is valid for 24
    /// hours from the time of generation.</remarks>
    /// <param name="user">The user for whom the JWT is being generated. The user's ID is included as a claim in the token.</param>
    /// <returns>A string representation of the generated JWT.</returns>
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

    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    /// <remarks>This method checks if the username already exists before attempting to register the user.  If
    /// the username is already in use, the method returns a bad request response with an appropriate error
    /// message.</remarks>
    /// <param name="registerDto">An object containing the user's registration details, including username and other required information.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the registration operation.  Returns <see
    /// cref="BadRequestObjectResult"/> if the username already exists, or <see cref="OkObjectResult"/> with a success
    /// message upon successful registration.</returns>
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


    /// <summary>
    /// Authenticates a user based on the provided credentials and generates a JWT token if successful.
    /// </summary>
    /// <remarks>This method verifies the provided username and password against stored user data. If the
    /// credentials are valid,  a JWT token is generated and returned in the response. Otherwise, an unauthorized
    /// response is returned.</remarks>
    /// <param name="request">An object containing the username and password for authentication.</param>
    /// <returns>An <see cref="IActionResult"/> containing an HTTP 200 response with a JWT token if authentication is successful,
    /// or an HTTP 401 response if the credentials are invalid.</returns>
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
