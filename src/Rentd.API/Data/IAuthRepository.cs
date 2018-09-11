using System.Threading.Tasks;
using Rentd.API.Models;

namespace Rentd.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Login(string username, string password);
         Task<User> Register(User user, string password);
         Task<bool> UserExist(string userName);
    }
}