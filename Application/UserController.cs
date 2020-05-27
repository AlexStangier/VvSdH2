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
        private User _loggedInAs;

        public async Task<bool> Login(DateTime timestampLogin, User user)
        {
            using var context = new ReservationContext();
            var foundUser = await context.Users.FirstOrDefaultAsync(x => x.Username.Equals(user.Username));

            if (foundUser == null)
            {
                // No user with that username in DB
                return false;
            }

            if (foundUser.Password != user.Password)
            {
                // Password doesn't match
                return false;
            }

            _loggedInAs = user;
            return true;
        }

        public async Task<bool> Logout()
        {
            if (_loggedInAs == null)
            {
                // can't log out because the user is not logged in.
                return false;
            }

            _loggedInAs = null;
            return true;
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