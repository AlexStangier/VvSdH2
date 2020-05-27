using System;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IUser
    {
        Task<bool> Login(DateTime timestampLogin, User user);
        Task<bool> Logout();
        Task<bool> CreateUser(User newUser);
        Task<bool> RemoveUser(User userToRemove);
    }
}