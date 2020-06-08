using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationShared;
using Core;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public sealed class BookingController : IBooking
    {
        private readonly IMail _mail;


        public static BookingController GetNoMailBookingController()
        {
            return new BookingController(new DummyMailController());
        }

        public BookingController(IMail mail)
        {
            _mail = mail;
        }

        public async Task<bool> CreateReservation(Room selectedRoom, DateTime timestamp, double duration, User user)
        {
            //Cannot Reservate in the past, accounting for lag
            if (timestamp < DateTime.Now.AddMinutes(-1))
                return false;

            await using var context = new ReservationContext();

            //Rangecheck input
            if (duration < 90) duration = 90;
            if (duration > 180) duration = 180;

            try
            {
                var existingReservation = await context.Reservations.Where(x =>
                        x.StartTime >= timestamp && x.EndTime <= timestamp.AddMinutes(duration)).Include(y => y.User)
                    .ThenInclude(z => z.Rights).FirstOrDefaultAsync();

                var concreteUser = await context.Users.FindAsync(user.Username);

                var isHoliday = await context.Holydays.Where(x =>
                    x.Date >= timestamp && x.Date <= timestamp.AddMinutes(duration)).FirstOrDefaultAsync();

                if (isHoliday != null || timestamp.DayOfWeek != DayOfWeek.Sunday)
                {
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
                            var success = await context.SaveChangesAsync() > 0;
                            if(success)
                            {
                                await _mail.SendConfirmationMail(concreteUser.Username);
                            }
                            return success;
                        }
                    }
                    else if (existingReservation.StartTime <= DateTime.Now.AddHours(24))
                    {
                        //Cannot overbook if reservation starts in 24 hours,
                        //no matter how priviledged the users are.
                        return false;
                    }
                    else if (await ComparePrivilege(concreteUser, existingReservation.User))
                    {
                        //Delete Reservation that has to be overbooked
                        context.Reservations.Remove(existingReservation);
                        await _mail.SendOverbookingMail(existingReservation.User.Username);

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
            }
            catch (NullReferenceException)
            {
                return false;
            }
            return false;
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

        public async Task<List<Reservation>> GetUserReservations(User user)
        {
            await using var context = new ReservationContext();
            
            var concreteUser = await context.Users.FindAsync(user.Username);

            return await context.Reservations.Where(x => x.User == concreteUser)
                                             .Where(x => x.EndTime >= DateTime.Now)
                                             .ToListAsync();
        }
    }
}