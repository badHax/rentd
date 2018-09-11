using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rentd.API.Data;
using Rentd.API.Models;

namespace Rentd.API.Controllers
{[Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo;

        public AuthController(IConfiguration config, IAuthRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        public async Task<IActionResult> Login(ViewModelLogin model){

           var userFromDatabase = await _repo.Login(model.UserName.ToLower(),model.Password);

           if(userFromDatabase==null)
           return Unauthorized();

           var claims = new[]{
               new Claim(ClaimTypes.NameIdentifier,userFromDatabase.Id),
               new Claim(ClaimTypes.Name,userFromDatabase.UserName)
           };

           var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
               _config.GetSection("AppSettings:Token").Value
           ));

           var signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.Sha512);

           var tokenDescriptor = new SecurityTokenDescriptor(){
               SigningCredentials = signingCredentials,
               Subject = new ClaimsIdentity(claims),
               Expires = DateTime.Now.AddDays(1)
           };

           var tokenManager = new JwtSecurityTokenHandler();

           var token = tokenManager.CreateJwtSecurityToken(tokenDescriptor);

           return Ok(new {
               token = tokenManager.WriteToken(token)
           });
        }

        public async Task<IActionResult> Register(ViewModelRegister model){

            if(await _repo.UserExist(model.UserName.ToLower()))
            return BadRequest($"The user {model.UserName} already exists.");
            
            var userToCreate = new User(){UserName=model.UserName.ToLower()};

            var createdUser = await _repo.Register(userToCreate,model.Password);

            if(createdUser == null)
            return BadRequest();

            return StatusCode(201);
        }
    }
}