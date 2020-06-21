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


        public static BookingController CreateBookingControllerNoMail()
        {
            return new BookingController(new DummyMailController());
        }

        public static BookingController CreateBookingController()
        {
            return new BookingController(new MailController());
        }

        public BookingController(IMail mail)
        {
            _mail = mail;
        }

        public async Task<bool> UpdateReservation(Reservation currReservation, DateTime newTime,
            int newSlot)
        {
            await using var context = new ReservationContext();

            var reservation = await context.Reservations
                .Where(x => x.ReservationId == currReservation.ReservationId)
                .Include(y => y.User)
                .ThenInclude(z => z.Rights)
                .FirstOrDefaultAsync();

            if (reservation == null) return false;
            var timestamp = getTimestampsFromTimeslot(newSlot, newTime);
            
            reservation.StartTime = timestamp.First();
            reservation.EndTime = timestamp.Last();
            return context.SaveChanges() > 0;
        }

        public async Task<bool> CreateReservation(Room selectedRoom, DateTime timestamp, int slot, User user)
        {
            //Cannot Reservate in the past, accounting for lag
            if (timestamp < DateTime.Now.AddMinutes(-1))
                return false;

            await using var context = new ReservationContext();

            var listTimes = getTimestampsFromTimeslot(slot, timestamp);

            try
            {
                var existingReservation = await context.Reservations.Where(x =>
                        x.StartTime >= listTimes.First() && x.EndTime <= listTimes.Last()).Include(y => y.User)
                    .ThenInclude(z => z.Rights).FirstOrDefaultAsync();

                var concreteUser = await context.Users.FindAsync(user.Username);

                var isHoliday =
                    await context.Holydays.AnyAsync(x =>
                        x.Date.Month == timestamp.Month && x.Date.Day == timestamp.Day);

                if (isHoliday || timestamp.DayOfWeek == DayOfWeek.Sunday)
                {
                    return false;
                }

                if (existingReservation == null)
                {
                    //Add new Reservation
                    if (concreteUser != null)
                    {
                        var newReservation = new Reservation
                        {
                            Room = await context.Rooms.FindAsync(selectedRoom.RoomId),
                            StartTime = listTimes.First(),
                            EndTime = listTimes.Last(),
                            User = concreteUser
                        };
                        context.Reservations.Add(newReservation);
                        concreteUser.Reservations.Add(newReservation);
                        var success = await context.SaveChangesAsync() > 0;
                        if (success)
                        {
                            await _mail.SendConfirmationMail(newReservation);
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
                    await _mail.SendOverbookingMail(existingReservation);

                    var newReservation = new Reservation
                    {
                        Room = await context.Rooms.FindAsync(selectedRoom.RoomId),
                        StartTime = listTimes.First(),
                        EndTime = listTimes.Last(),
                        User = concreteUser
                    };

                    //Create new Reservation
                    context.Reservations.Add(newReservation);
                    concreteUser.Reservations.Add(newReservation);
                    return await context.SaveChangesAsync() > 0;
                }

                return false;
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
            var fittingReservation = await context.Reservations.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ReservationId == Id);

            // Check if cancelling is possible
            if (fittingReservation == null)
                return false;

            if (!fittingReservation.User.Equals(user))
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

        private List<DateTime> getTimestampsFromTimeslot(int slot, DateTime selectedDay)
        {
            var toReturn = new List<DateTime>();
            var newStartDate = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day);
            var newEndDate = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day);

            switch (slot)
            {
                case 1:
                    newStartDate = newStartDate.AddHours(8).AddMinutes(0);
                    newEndDate = newEndDate.AddHours(9).AddMinutes(30);
                    break;
                case 2:
                    newStartDate = newStartDate.AddHours(9).AddMinutes(45);
                    newEndDate = newEndDate.AddHours(11).AddMinutes(15);
                    break;
                case 3:
                    newStartDate = newStartDate.AddHours(11).AddMinutes(35);
                    newEndDate = newEndDate.AddHours(13).AddMinutes(05);
                    break;
                case 4:
                    newStartDate = newStartDate.AddHours(14).AddMinutes(0);
                    newEndDate = newEndDate.AddHours(15).AddMinutes(30);
                    break;
                case 5:
                    newStartDate = newStartDate.AddHours(15).AddMinutes(45);
                    newEndDate = newEndDate.AddHours(17).AddMinutes(15);
                    break;
                case 6:
                    newStartDate.AddHours(17).AddMinutes(30);
                    newEndDate = newEndDate.AddHours(19).AddMinutes(00);
                    break;
                default:
                    throw new Exception("Slot index was out of Range");
            }

            toReturn.Add(newStartDate);
            toReturn.Add(newEndDate);

            return toReturn;
        }
    }
}