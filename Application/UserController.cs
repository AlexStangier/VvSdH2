using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application
{
    public class UserController
    {
        private User _loggedInAs;

        public async Task<bool> login(DateTime timestampLogin, User user)
        {
            var context = new ReservationContext();
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

        public async Task<bool> logout()
        {
            if (_loggedInAs == null)
            {
                // can't log out because the user is not logged in.
                return false;
            }

            _loggedInAs = null;
            return true;
        }
    }
}