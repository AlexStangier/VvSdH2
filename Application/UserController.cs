using System;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application
{
    public class UserController
    {
        // TODO add the timeout functionality

        private object _userLock = new object();
        private User _loggedInAs;

        public async Task<bool> login(DateTime timestampLogin, User user)
        {
            lock (_userLock)
            {
                var rc = new ReservationContext();
                var foundUser = rc.Users.Find(user.Username);

                if(foundUser == null)
                {
                    // No user with that username in DB
                    return false;
                }

                if(foundUser.Password != user.Password)
                {
                    // Password doesn't match
                    return false;
                }

                _loggedInAs = user;
                return true;
            }
        }

        public async Task<bool> logout()
        {
            lock (_userLock)
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
}