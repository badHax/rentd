using System.Threading.Tasks;
using Rentd.API.Models;

namespace Rentd.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        public Task<User> Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Register(User user, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UserExist(string userName)
        {
            throw new System.NotImplementedException();
        }
    }
}