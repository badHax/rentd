using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Rentd.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        //testing
        [HttpGet("dashboard")]
        [Authorize(Policy = "api1_access")]
        public IEnumerable<string> Dashboard()
        {
            return new string[] { "profile info"};
        }
    }
}