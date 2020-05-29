using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IBooking
    {
        Task<bool> CreateReservation(Room selectedRoom, DateTime timestamp, double duration, User user);
        Task<bool> CancelReservation(User user, int Id);
        Task<List<Reservation>> GetUserReservations(User user);
    }
}