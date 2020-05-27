using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationShared;
using Core;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public class BookingController : IBooking
    {
        public async Task<bool> CreateReservation(Room selectedRoom, DateTime timestamp, double duration, User user)
        {
            await using var context = new ReservationContext();
            try
            {
                var existingReservation = await context.Reservations.Where(x =>
                    x.StartTime >= timestamp && x.EndTime <= timestamp.AddMinutes(duration)).FirstOrDefaultAsync();

                var concreteUser = await context.Users.FindAsync(user.Username);


                if (existingReservation == null)
                {
                    if (concreteUser != null)
                    {
                        var newReservation = new Reservation
                        {
                            Room = await context.Rooms.FindAsync(selectedRoom.RoomId),
                            StartTime = timestamp,
                            EndTime = timestamp.AddMinutes(duration),
                            User = concreteUser
                        };
                        context.Reservations.Add(newReservation);
                        concreteUser.Reservations.Add(newReservation);
                        return await context.SaveChangesAsync() > 0;
                    }
                }
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public async Task<bool> CancelReservation(User user, int Id)
        {
            await using var context = new ReservationContext();
            var fittingReservation = await context.Reservations.FirstOrDefaultAsync(x => x.ReservationId == Id);

            // Check if cancelling is possible
            if (fittingReservation == null)
                return false;

            if (fittingReservation.User != user)
                return false;

            // Cancelling is possible, remove the entry
            context.Reservations.Remove(fittingReservation);
            context.SaveChanges();
            return true;
        }
    }
}