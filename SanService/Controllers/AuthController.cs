using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SanatoriumCore.Secure;
using SanatoriumEntities.Entities;
using SanatoriumEntities.Models.Services;
using SanService.Infrastructure;
using SanService.Models;
using SanService.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        UserService _userservice = new UserService();

        public AuthController(IConfiguration config)
        {
            _config = config;
        }


        [ApiKeyAuth("sid", Access = AccessLevel.Admin)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {          

            
            var userVeri = _userservice.LoginUser(request.Username, request.Password);

            if (!userVeri.Success)
            {
                return Unauthorized();                
            }

            //generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name, userVeri.Data.FirstName.ToString()),
                    new Claim(ClaimTypes.Surname, userVeri.Data.LastName.ToString()),
                    new Claim(ClaimTypes.Role, userVeri.Data.RoleID.ToString()),                    
                    new Claim(ClaimTypes.GivenName, userVeri.Data.Patronymic.ToString()),
                    new Claim(ClaimTypes.Sid, userVeri.Data.SessionID.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userVeri.Data.EmployeeId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });
        }

        [HttpGet("session")]
        public async Task<IActionResult> Session()
        {
            var claimsIndentity = this.HttpContext.User.Identity as ClaimsIdentity;
            UserSid userSid = new UserSid
            {
                name = claimsIndentity.Name
            };

            foreach (var claimsIndentityClaim in claimsIndentity.Claims)
            {
                if (claimsIndentityClaim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")
                {
                    userSid.sid = claimsIndentityClaim.Value;
                }
                if (claimsIndentityClaim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                {
                    userSid.id = Convert.ToInt32(claimsIndentityClaim.Value);
                }
                if (claimsIndentityClaim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")
                {
                    userSid.givenname = claimsIndentityClaim.Value;
                }
                if (claimsIndentityClaim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")
                {
                    userSid.surname = claimsIndentityClaim.Value;
                }
            }

            var resultEmp = _userservice.GetEmpGenToid(userSid.id);
            if (resultEmp.Success)
            {
                userSid.empGenId = resultEmp.Data.id.ToString();
            }

            return Ok(userSid);
        }
    }
}
