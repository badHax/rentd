using System.Threading.Tasks;
using Rentd.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Rentd.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApiDbContext _context;
        public AuthRepository(ApiDbContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            if(!await UserExist(username))
            return null;

            var userFromDatabase = await _context.User.FirstOrDefaultAsync(
                u => u.UserName.Equals(username)
            );

            if(!VerifyPasswordHash(password,userFromDatabase.PasswordHash,userFromDatabase.PasswordSalt))
            return null;

            return userFromDatabase;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            HashUserPassword(password,out passwordHash,out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExist(string userName)
        {
            var userFromDb = await _context.User.FirstOrDefaultAsync(
                u => u.UserName.Equals(userName)
            );
            return userFromDb != null;
        }

        private void HashUserPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hash512 = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hash512.Key;
                passwordHash = hash512.ComputeHash(Encoding.UTF8.GetBytes(password));
            }  
        }

        private bool VerifyPasswordHash (string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hash512 = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var passwordAttempt = hash512.ComputeHash(Encoding.UTF8.GetBytes(password));
                return passwordHash.SequenceEqual(passwordAttempt);
            }
        }
    }
}