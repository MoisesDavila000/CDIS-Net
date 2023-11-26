using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.DTOs;
using BankAPI.Data.BankModels;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginService _loginService;
    private readonly IConfiguration _config;

    public LoginController(LoginService loginService, IConfiguration config)
    {
        _loginService = loginService;
        _config = config;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Login(AdminDTO adminDTO)
    {
        var admin = await _loginService.GetAdmin(adminDTO);

        if(admin is null)
            return BadRequest(new { message = "Credenciales invalidas."});

        string jwtToken = GenerateToken(admin);
        return Ok( new {token = jwtToken});
    }

    [HttpPost("autenticateClient")]
    public async Task<IActionResult> LoginClient(ClientDTO clientDTO)
    {
        var client = await _loginService.GetClient(clientDTO);

        if(client is null)
            return BadRequest(new { message ="Credenciales invalidas."});

        string jwtToken = GenerateClientToken(client);
        return Ok( new {token = jwtToken});
    }

    private string GenerateToken(Administrator admin)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("AdminType", admin.AdminType)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds
        );

        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;

    }

    private string GenerateClientToken(Client client)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Email, client.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds
        );

        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;

    }
}