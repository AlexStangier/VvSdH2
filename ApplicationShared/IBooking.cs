using System;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IBooking
    {
        Task<bool> reservation(Room selectedRoom, DateTime timestamp, User user);
        Task<bool> cancelReservation(User user, int Id);
    }
}