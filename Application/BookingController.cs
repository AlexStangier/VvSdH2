using System;
using System.Threading.Tasks;
using Core;

namespace Application
{
    public class BookingController
    {
        public async Task<bool> reservation(DateTime timestamp, User user)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task<bool> cancelReservation(User user, int Id)
        {
            var rc = new ReservationContext();
            var fittingReservation = rc.Reservations.Find(Id);

            // Check if cancelling is possible
            if (fittingReservation == null)
                return false;

            if (fittingReservation.User != user)
                return false;

            // Cancelling is possible, remove the entry
            rc.Reservations.Remove(fittingReservation);
            rc.SaveChanges();
            return true;
        }
    }
}