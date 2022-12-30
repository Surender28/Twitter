
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Twitter_new.DTOs;
using Twitter_new.Models;
using Twitter_new.Repositories;
using Twitter_new.Utilities;

namespace Twitter_new.Controllers;
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _user;
    private readonly IConfiguration _config;


    public UserController(ILogger<UserController> logger, IUserRepository User, IConfiguration config)
    {
        _logger = logger;
        _user = User;
        _config = config;

    }


    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserCreateDTO Data)
    {
        var existingUser = await _user.FindEmail(Data.Email);
        if (existingUser != null)
            return BadRequest("Email Is Already Taken Try With Another Email ");
        var toCreateUser = new User
        {
            Fullname = Data.FullName.Trim(),
            Email = Data.Email.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(Data.Password),
        };

        var createdUser = await _user.CreateUser(toCreateUser);

        return StatusCode(StatusCodes.Status201Created, createdUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResDTO>> Login(
        [FromBody] UserLoginDTO Data
    )
    {

        var existingUser = await _user.FindEmail(Data.Email);

        if (existingUser is null)
            return NotFound("User Not Found With Current Email Address");


        if (!BCrypt.Net.BCrypt.Verify(Data.Password, existingUser.Password))
            return BadRequest("Incorrect password");
        var token = Generate(existingUser);

        var res = new UserLoginResDTO
        {
            Id = existingUser.Id,
            FullName = existingUser.Fullname,
            Email = existingUser.Email,

            Token = token,
        };

        return Ok(res);
    }

    private string Generate(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(UserConstants.Id, user.Id.ToString()),
            new Claim(UserConstants.FullName, user.Fullname),
            new Claim(UserConstants.Email, user.Email),
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<ActionResult> UpdateUser([FromBody] UserUpdateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);
        var existing = await _user.GetUserById(userId);
        if (existing is null)
            return NotFound("No User found with given User Id");

        var toUpdateUser = existing with
        {

            Fullname = Data.FullName ?? existing.Fullname,

        };

        var didUpdate = await _user.UpdateUser(toUpdateUser);

        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not update FullName");

        return NoContent();
    }


}


