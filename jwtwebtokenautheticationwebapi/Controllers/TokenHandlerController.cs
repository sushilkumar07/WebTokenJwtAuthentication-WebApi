using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace jwtwebtokenautheticationwebapi.Controllers
{
    public class TokenHandlerController : Controller
    {
        private IConfiguration _config;

        public TokenHandlerController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/Login/{username}/{password}")]
        public IActionResult Get(string username, string password)
        {
            if (username == password)
            {
                return new ObjectResult(GenerateJSONWebToken(new UserModel
                { Username = "sushilkumar", EmailAddress = "sushil.shinde@gmail.com" }));
            }
            else
                return BadRequest();

        }

        //[HttpGet]
        //[Route("api/Login")]
        //public IActionResult Login([FromBody]UserModel login)
        //{
        //    IActionResult response = Unauthorized();
        //    UserModel user = AuthenticateUser(login);

        //    if (user != null)
        //    {
        //        var tokenString = GenerateJSONWebToken(user);
        //        response = Ok(new { token = tokenString });
        //    }

        //    return response;
        //}

        private string GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel AuthenticateUser(UserModel login)
        {
            UserModel user = null;

            //Validate the User Credentials  
            //Demo Purpose, I have Passed HardCoded User Information  
            if (login.Username == "sushilkumar")
            {
                user = new UserModel { Username = "sushilkumar", EmailAddress = "sushil.shinde@gmail.com" };
            }
            return user;
        }
    }
}
