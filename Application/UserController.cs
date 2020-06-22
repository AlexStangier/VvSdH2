using System;
using System.Threading.Tasks;
using ApplicationShared;
using Core;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public sealed class UserController : IUser
    {
        public async Task<int> Login(string username, string password)
        {
            using var context = new ReservationContext();
            var foundUser = await context.Users.FirstOrDefaultAsync(x => x.Username.Equals(username));
            if (foundUser?.Password.Equals(password) ?? false)
            {
                if (foundUser.HasCurrentSession)
                {
                    return 1;
                }

                foundUser.HasCurrentSession = true;
                context.SaveChanges();

                if (foundUser?.HasCurrentSession ?? false)
                {
                    return 2;
                }
            }

            return 0;
        }

        public async Task<bool> Logout(string username)
        {
            using var context = new ReservationContext();

            var currentUser = await context.Users.FirstOrDefaultAsync(x => x.Username.Equals(username));

            if (currentUser == null) return false;

            currentUser.HasCurrentSession = false;

            context.SaveChanges();

            //Default for HasCurrentSession should be false
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