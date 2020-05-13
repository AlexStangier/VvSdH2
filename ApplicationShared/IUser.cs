using System;
using Core;

namespace ApplicationShared
{
    public interface IUser
    {
        bool login(DateTime timestampLogin, User user);
        bool logout();
    }
}