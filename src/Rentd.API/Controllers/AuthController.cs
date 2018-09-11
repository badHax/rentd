using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
            throw new System.NotImplementedException();
        }

        public async Task<IActionResult> Register(ViewModelRegister model){
            throw new System.NotImplementedException();
        }
    }
}