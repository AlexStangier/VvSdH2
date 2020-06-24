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

            reservation.StartTime = timestamp.start;
            reservation.EndTime = timestamp.end;
            return context.SaveChanges() > 0;
        }

        public async Task<int> CreateReservation(Room selectedRoom, DateTime timestamp, int slot, User user)
        {
            if (selectedRoom == null)
                return -4;

            var _mail = new MailController();

            await using var context = new ReservationContext();

            var listTimes = getTimestampsFromTimeslot(slot, timestamp);

            //Cannot Reservate in the past, accounting for lag
            if (listTimes.end < DateTime.Now.AddMinutes(-1))
                return -1;

            try
            {
                var existingReservation = await context.Reservations
                    .Where(x => x.StartTime >= listTimes.start && x.EndTime <= listTimes.end)
                    .Where(x => x.Room == selectedRoom)
                    .Include(u => u.Room)
                    .Include(y => y.User)
                    .ThenInclude(z => z.Rights).FirstOrDefaultAsync();

                var concreteUser = await context.Users.FindAsync(user.Username);

                var isHoliday =
                    await context.Holydays.AnyAsync(x =>
                        x.Date.Month == timestamp.Month && x.Date.Day == timestamp.Day);

                if (isHoliday || timestamp.DayOfWeek == DayOfWeek.Sunday)
                {
                    return -2;
                }

                if (existingReservation == null)
                {
                    //Add new Reservation
                    if (concreteUser != null)
                    {
                        var newReservation = new Reservation
                        {
                            Room = await context.Rooms.FindAsync(selectedRoom.RoomId),
                            StartTime = listTimes.start,
                            EndTime = listTimes.end,
                            User = concreteUser
                        };
                        context.Reservations.Add(newReservation);
                        concreteUser.Reservations.Add(newReservation);
                        var success = await context.SaveChangesAsync() > 0;
                        if (success)
                        {
                            if (await _mail.SendConfirmationMail(newReservation))
                                return 1;

                            return -3;
                        }

                        return 0;
                    }
                }
                else if (existingReservation.StartTime <= DateTime.Now.AddHours(24))
                {
                    //Cannot overbook if reservation starts in 24 hours,
                    //no matter how priviledged the users are.
                    return -4;
                }
                else if (await ComparePrivilege(concreteUser, existingReservation.User))
                {
                    //Delete Reservation that has to be overbooked
                    context.Reservations.Remove(existingReservation);

                    var newReservation = new Reservation
                    {
                        Room = await context.Rooms.FindAsync(selectedRoom.RoomId),
                        StartTime = listTimes.start,
                        EndTime = listTimes.end,
                        User = concreteUser
                    };

                    //Create new Reservation
                    context.Reservations.Add(newReservation);
                    concreteUser.Reservations.Add(newReservation);
                    var success = await context.SaveChangesAsync() > 0;
                    if (success)
                    {
                        if (await _mail.SendOverbookingMail(existingReservation) && await _mail.SendConfirmationMail(newReservation))
                            return 1;

                        return -3;
                    }
                }

                return 0;
            }
            catch (NullReferenceException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Comapares 2 Users by their Privileges return true if user A is higher prioritzed than B
        /// </summary>
        /// <param name="userA">User object</param>
        /// <param name="userB">User object</param>
        /// <returns></returns>
        public async Task<bool> ComparePrivilege(User userA, User userB)
        {
            await using var context = new ReservationContext();
            
            var concreteUserA = await context.Users
                .Where(x => x.Username.Equals(userA.Username))
                .Include(y => y.Rights)
                .Select(s => s.Rights.PrivilegeLevel)
                .FirstOrDefaultAsync(); 

            var concreteUserB = await context.Users
                .Where(x => x.Username.Equals(userB.Username))
                .Include(y => y.Rights)
                .Select(s => s.Rights.PrivilegeLevel)
                .FirstOrDefaultAsync();

            return concreteUserA > concreteUserB;
        }

        public async Task<bool> CancelReservation(User user, int Id)
        {
            await using var context = new ReservationContext();
            
            var userFromReservation = await context.Reservations
                .Include(x => x.User)
                .ThenInclude(r => r.Rights)
                .FirstOrDefaultAsync(x => x.ReservationId == Id);

            var currentUser = await context.Users
                .Where(x => x.Username.Equals(user.Username))
                .Include(y => y.Rights)
                .Select(s => s.Rights.PrivilegeLevel)
                .FirstOrDefaultAsync();

            // Check if cancelling is possible
            if (userFromReservation == null)
                return false;

            var getFittingPrivilege = userFromReservation.User.Rights.PrivilegeLevel;
            
            if (currentUser < getFittingPrivilege && currentUser < 4)
                return false;

            if (currentUser == getFittingPrivilege && !userFromReservation.User.Equals(user) && currentUser < 4)
                return false;

            // Cancelling is possible, remove the entry
            context.Reservations.Remove(userFromReservation);
            return context.SaveChanges() > 0;
        }

        public async Task<List<Reservation>> GetUserReservations(User user)
        {
            await using var context = new ReservationContext();

            var concreteUser = await context.Users.FindAsync(user.Username);

            return await context.Reservations.Where(x => x.User == concreteUser)
                                             .Include(x => x.Room)
                                             .ToListAsync();
        }

        public async Task<List<Reservation>> GetAllReservations()
        {
            await using var context = new ReservationContext();

            return await context.Reservations.Include(x => x.User)
                                             .Include(x => x.Room)
                                             .ToListAsync();
        }

        public (DateTime start, DateTime end) getTimestampsFromTimeslot(int slot, DateTime selectedDay)
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
                    newStartDate = newStartDate.AddHours(17).AddMinutes(30);
                    newEndDate = newEndDate.AddHours(19).AddMinutes(00);
                    break;
                default:
                    throw new Exception("Slot index was out of Range");
            }

            return (newStartDate, newEndDate);
        }

        public List<Reservation> RemovePastReservations(List<Reservation> reservations)
        {
            var now = DateTime.Now;

            return reservations.Where(x => x.EndTime >= now).ToList();
        }
    }
}