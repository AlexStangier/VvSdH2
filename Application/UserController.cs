using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationShared;
using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application
{
    public class UserController : IUser
    {
        public async Task<bool> Login(string username, string password)
        {
            using var context = new ReservationContext();
            var foundUser = await context.Users.FirstOrDefaultAsync(x => x.Username.Equals(username));
            if (foundUser?.Password.Equals(password) ?? false)
            {
                foundUser.HasCurrentSession = true;
                context.SaveChanges();

                return foundUser.HasCurrentSession;
            }

            return false;
        }

        public async Task<bool> Logout(string username)
        {
            using var context = new ReservationContext();

            var currentUser = await context.Users.FirstOrDefaultAsync(x => x.Username.Equals(username));

            currentUser.HasCurrentSession = false;

            context.SaveChanges();

            return !currentUser.HasCurrentSession;
        }

        public Task<bool> CreateUser(User newUser)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUser(User userToRemove)
        {
            throw new NotImplementedException();
        }
    }
}