using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Service.DTOs;
using Auth.Service.Model;
using Auth.Service.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request -- Below code needs to be used incase we dont use [ApiController] attribute
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            if (_repo.UserExists(userForRegisterDto.username.ToLower(), userForRegisterDto.role.ToLower())) return BadRequest("Username already exists");

            userForRegisterDto.username = userForRegisterDto.username.ToLower();

            var userToCreate = new User {
                Username = userForRegisterDto.username,
                Role = userForRegisterDto.role
            };

            var createdUser = _repo.Register(userToCreate, userForRegisterDto.password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password, userForLoginDto.role.ToLower());

            if (userFromRepo == null)
                return Unauthorized();

            //Token Generation
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //A security key is created using the secret from the appsettings.Development.json file
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //The created key is encrypted using a security algorithm
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            //The token details are specified using the SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = creds
            };

            //The token handler instance is created
            var tokenHandler = new JwtSecurityTokenHandler();

            //The token is created using the token handler
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                username = userFromRepo.Username,
                role = userFromRepo.Role,
                //The token is writen into the request using the token handler
                token = tokenHandler.WriteToken(token),
                tokenExpirationDate = tokenDescriptor.Expires
            });
        }
    }
}