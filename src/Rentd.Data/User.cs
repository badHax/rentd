///////////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

///////////////////////////////////////////////////////

namespace Rentd.Data
{
    public class User : IdentityUser
    {
        public List<Claim> Claims { get; set; }
    }
}
