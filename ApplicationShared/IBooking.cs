using System;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IBooking
    {
        Task<bool> CreateReservation(Room selectedRoom, DateTime timestamp, User user);
        Task<bool> CancelReservation(User user, int Id);
    }
}