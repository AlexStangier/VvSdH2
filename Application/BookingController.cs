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

            //Rangecheck input
            if (duration < 90) duration = 90;
            if (duration > 180) duration = 180;

            try
            {
                var concreteUser = await context.Users.FindAsync(user.Username);

                var isHolyday = await context.Holydays.Where(x =>
                    x.Date >= timestamp && x.Date <= timestamp.AddMinutes(duration)).FirstOrDefaultAsync();

                if (isHolyday != null || (timestamp.DayOfWeek != DayOfWeek.Sunday))
                {
                    var existingReservation = await context.Reservations.Where(x =>
                            x.StartTime >= timestamp && x.EndTime <= timestamp.AddMinutes(duration)).Include(y => y.User)
                        .ThenInclude(z => z.Rights).FirstOrDefaultAsync();

                    if (existingReservation == null)
                    {
                        //Add new Reservation
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
                    else if (await ComparePrivilege(concreteUser, existingReservation.User))
                    {
                        //Delete Reservation that has to be overbooked
                        context.Reservations.Remove(existingReservation);

                        var newReservation = new Reservation
                        {
                            Room = await context.Rooms.FindAsync(selectedRoom.RoomId),
                            StartTime = timestamp,
                            EndTime = timestamp.AddMinutes(duration),
                            User = concreteUser
                        };

                        //Create new Reservation
                        context.Reservations.Add(newReservation);
                        concreteUser.Reservations.Add(newReservation);
                        return await context.SaveChangesAsync() > 0;
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        /// <summary>
        /// Comapares 2 Users by their Privileges return true if user A is higher prioritzed than B
        /// </summary>
        /// <param name="userA">User object</param>
        /// <param name="userB">User object</param>
        /// <returns></returns>
        private async Task<bool> ComparePrivilege(User userA, User userB)
        {
            await using var context = new ReservationContext();
            var concreteUserA = await context.Users.Where(x => x.Username.Equals(userA.Username)).Include(y => y.Rights)
                .FirstOrDefaultAsync();
            var concreteUserB = await context.Users.Where(x => x.Username.Equals(userB.Username)).Include(y => y.Rights)
                .FirstOrDefaultAsync();

            return concreteUserA.Rights.PrivilegeLevel > concreteUserB.Rights.PrivilegeLevel;
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
            return context.SaveChanges() > 0;
        }
    }
}