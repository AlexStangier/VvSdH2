using System;
using Core;

namespace ApplicationShared
{
    public interface IBooking
    {
        bool reservation(DateTime timestamp, User user);
        bool cancelReservation(User user, int Id);
    }
}