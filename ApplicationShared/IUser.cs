using System;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IUser
    {
        Task<bool> login(DateTime timestampLogin, User user);
        Task<bool> logout();
    }
}