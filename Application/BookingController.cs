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
            var context = new ReservationContext();
            var currUser = await context.Users.Where(x => x.Username.Equals(user.Username)).Include(y => y.Rights)
                .FirstOrDefaultAsync();

            //Check for existing Reservations
            var possibleReservation = await context.Reservations.Where(x => x.Start.Equals(timestamp))
                .Include(y => y.User).ThenInclude(z => z.Rights).FirstOrDefaultAsync();
            if (possibleReservation != null)
            {
                
            }

            return false;
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