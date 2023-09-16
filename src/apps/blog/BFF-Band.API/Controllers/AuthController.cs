using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BFF_Band.API.Models;
using BFF_Band.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BFF_Band.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController: ControllerBase
{
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        private User _userTest;
        public AuthController(IConfiguration configuration, IUserService userService, IJwtService jwtService)
        {
            _configuration = configuration;
            _userService = userService;
            _jwtService = jwtService;


            jwtService.CreatePasswordHash("123", out byte[] passwordHashTest, out byte[] passwordSaltTest);
            _userTest = CreateTestUser("admin", passwordHashTest, passwordSaltTest);
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            _jwtService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (_userTest.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!_jwtService.VerifyPasswordHash(request.Password, _userTest.PasswordHash, _userTest.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = _jwtService.CreateToken(_userTest);

            var refreshToken = _jwtService.GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if(user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = _jwtService.CreateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private User CreateTestUser(string username, byte[] passwordHash, byte[] passwordSalt)
        {
            var output = new User();
            output.Username = username;
            output.PasswordHash = passwordHash;
            output.PasswordSalt = passwordSalt;
            return output;
        }
}