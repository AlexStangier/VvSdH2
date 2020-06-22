using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IUser
    {
        Task<int> Login(string Username, string Password);
        Task<bool> Logout(string username);
        Task<bool> CreateUser(User newUser);
        Task<bool> RemoveUser(User userToRemove);
    }
}