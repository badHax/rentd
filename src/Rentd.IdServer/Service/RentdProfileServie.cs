using Microsoft.AspNetCore.Identity;
using Rentd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rentd.IdServer
{
    public class RentdProfileServie : AspNetIdentityProfileService<User>
    {
        private readonly IdContext _context;

        public RentdProfileServie(UserManager<User> userManager, IdContext context) : base(userManager)
        {
            _context = context;    
        }

        /// <summary>
        /// Here we can grab whatever uniquely identifies the type of user profile we are tring to find
        /// We can implement this class for all the different types of user profiles we have in the application
        /// </summary>
        /// <param name="user">generic user</param>
        /// <returns>claims for this user specific that matches this profile</returns>
        protected override async Task<List<Claim>> GetIdentityClaims(User user)
        {
            var userData = _context.Users.
                Where(u => u.Id == user.Id);

            if (userData == null)
            {
                throw new Exception($"user with id {user.Id} not found");
            }

            //add profile specific claims here
            var claims = new List<Claim>();

            //add the base claims for user also
            claims.AddRange(await base.GetIdentityClaims(user));

            return claims;
        }
    }
}
