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
        public async Task<bool> reservation(Room selectedRoom, DateTime timestamp, User user)
        {
            await using var context = new ReservationContext();
            throw new NotImplementedException();
        }

        public async Task<bool> cancelReservation(User user, int Id)
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