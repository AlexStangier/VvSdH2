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
        public async Task<bool> reservation(DateTime timestamp, User user)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task<bool> cancelReservation(User user, int Id)
        {
            var context = new ReservationContext();
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