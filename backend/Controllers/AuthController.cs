using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;

namespace backend.Controllers;

[ApiController]
[Route("api/auth")]
[AutoValidateAntiforgeryToken]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        string role;

        if (dto.Username == _configuration["Admin:Username"] &&
            dto.Password == _configuration["Admin:Password"])
        {
            role = "Admin";
        }
        else if (dto.Username == _configuration["Manager:Username"] &&
                 dto.Password == _configuration["Manager:Password"])
        {
            role = "Manager";
        }
        else
        {
            return Unauthorized("Invalid username or password.");
        }

        var claims = new List<Claim>
        // A claim is a piece of information about an authenticated user.
    {
        new Claim(ClaimTypes.Name, dto.Username),
        new Claim(ClaimTypes.Role, role)
        // Individual information about the user
    };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);
        // Represents the user within the application

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return Ok(new { username = dto.Username, role });
    }

    [HttpGet("me")]
    [Authorize] // ensures that only authenticated users can access this endpoint
    public IActionResult Me()
    {
        return Ok(new
        {
            username = User.Identity?.Name, // User represents the currently authenticated user
            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }

    [HttpGet("csrf")]
    public IActionResult GetCsrfToken([FromServices] IAntiforgery antiforgery)
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new { token = tokens.RequestToken });
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return NoContent();
    }
}