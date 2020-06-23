using System;
using System.Linq;
using System.Threading.Tasks;
using Application;
using ApplicationShared;
using Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests
{
    public class ControllerTests
    {
        private IUser _user;
        private IBooking _booking;
        private IRoom _room;
        private Room dummyRoom;
        private User dummyUser;
        private DateTime testDate;


        public ControllerTests()
        {
            SetUp();
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _user = new UserController();
            _booking = new BookingController();
            _room = new RoomController();

            dummyRoom = new Room();
            dummyUser = new User();

            using var context = new ReservationContext();

            var _testDate = new DateTime(2040, 10, 15);
            testDate = _testDate;
        }

        /**
         * COMPONENTTESTS
         */
        [Test]
        public async Task TryReturnWholeFloor()
        {
            var rooms = await _room.GetFloor(1, "A");

            Assert.AreEqual(10, rooms.Count);
        }

        [Test]
        public async Task TryFilterRoomsBySize()
        {
            var rooms = await _room.GetFloor(1, "A");
            var filteredRooms = await _room.Filter(rooms, 50, new Core.Attribute());

            Assert.AreEqual(3, filteredRooms.Count);
        }

        [Test]
        public async Task TryStandartLogin()
        {
            var user = new User
            {
                Username = "udo@hs-offenburg.de",
                Password = "34cx324"
            };
            var result = await _user.Login(user.Username, user.Password);

            // User exists, PW is correct
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task TryInvalidLoginWrongPassword()
        {
            var user = new User
            {
                Username = "udo@hs-offenburg.de",
                Password = "12345"
            };

            var result = await _user.Login(user.Username, user.Password);

            // User exists, PW is wrong
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task TryLoginWithoutCorrespondingUser()
        {
            var user = new User
            {
                Username = "",
                Password = ""
            };
            var result = await _user.Login(user.Username, user.Password);

            // User does not exist
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task TryStandartLogout()
        {
            Assert.True(await _user.Logout("udo@hs-offenburg.de"));
        }

        [Test]
        public async Task TryLogoutWithInvalidCredentials()
        {
            Assert.False(await _user.Logout(""));
        }

        [Test]
        public async Task TryGetRoomByBuildingAndNumber()
        {
            Assert.NotNull(await _room.GetCurrentStatusForRoom(100, "A"));
        }

        [Test]
        public async Task TryGetRoomWithInvalidRoomNumber()
        {
            Assert.Null(await _room.GetCurrentStatusForRoom(0, "A"));
        }

        [Test]
        public async Task TryGetRoomWithInvalidBuilding()
        {
            Assert.Null(await _room.GetCurrentStatusForRoom(100, ""));
        }

        [Test]
        public async Task TryGetFloorByBuildingAndNumber()
        {
            Assert.NotNull(await _room.GetCurrentStatusForFloor("A", 1));
        }

        [Test]
        public void TryGetFloorWithInvalidRoomNumber()
        {
            Assert.AreEqual(0, _room.GetCurrentStatusForFloor("A", 0).Result.Count);
        }

        [Test]
        public void TryGetFloorWithInvalidBuilding()
        {
            Assert.AreEqual(0, _room.GetCurrentStatusForFloor("", 1).Result.Count);
        }

        [Test]
        public async Task TryCreateReservationWithInvalidRoom()
        {
            var time = DateTime.Now;
            var newEndDate = new DateTime(time.Year, time.Month, time.Day);
            newEndDate = newEndDate.AddDays(1).AddHours(9).AddMinutes(30);

            if (time.DayOfWeek == DayOfWeek.Sunday) time.AddDays(1);
            Assert.AreEqual(-4, await _booking.CreateReservation(null, newEndDate, 1, dummyUser));
        }

        [Test]
        public async Task TryCreateReservationWithInvalidUser()
        {
            var time = DateTime.Now;
            var newEndDate = new DateTime(time.Year, time.Month, time.Day);
            newEndDate = newEndDate.AddDays(1).AddHours(9).AddMinutes(30);

            if (time.DayOfWeek == DayOfWeek.Sunday) time.AddDays(1);
            Assert.AreEqual(0, await _booking.CreateReservation(dummyRoom, newEndDate, 1, null));
        }

        [Test]
        public async Task TryCreateRerservation()
        {
            using var context = new ReservationContext();
            Assert.AreEqual(1, await _booking.CreateReservation(await context.Rooms.FindAsync(1), testDate, 1,
                await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        /**[Test]
        public async Task TryOverbookReservation()
        {
            using var context = new ReservationContext();
            Assert.True(await _booking.CreateReservation(await context.Rooms.FindAsync(1), testDate, 1,
                await context.Users.FindAsync("udo@hs-offenburg.de")));
        }**/
        [Test]
        public async Task TrySundayRerservation()
        {
            using var context = new ReservationContext();
            Assert.AreEqual(-2, await _booking.CreateReservation(await context.Rooms.FindAsync(1),
                new DateTime(2020, 8, 30, 12, 0, 0), 1, await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        [Test]
        public async Task TryHolidayRerservation()
        {
            using var context = new ReservationContext();
            Assert.AreEqual(-2, await _booking.CreateReservation(await context.Rooms.FindAsync(1),
                new DateTime(2020, 12, 26, 12, 0, 0), 1, await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        [Test]
        public async Task GetReservationsForSpecifiedUser()
        {
            using var context = new ReservationContext();
            Assert.NotNull(_booking.GetUserReservations(await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        [Test]
        public async Task GetReservationsForInvalidUser()
        {
            var result = await _booking.GetUserReservations(dummyUser);
            Assert.GreaterOrEqual(1, result?.Count ?? 0);
        }

        [Test]
        public async Task UpdateReservation()
        {
            using var context = new ReservationContext();
            if (context.Reservations.Count() > 0)
            {
                var update = await _booking.UpdateReservation(
                    context.Reservations.OrderByDescending(x => x.ReservationId).FirstOrDefault(),
                    new DateTime(2020, 12, 20), 2);
                Assert.IsTrue(update);
            }

            Assert.IsTrue(true);
        }


        [Test]
        public async Task TestComparePrivilegeOfInvalidUsers()
        {
            Assert.IsFalse(await _booking.ComparePrivilege(new User(), new User()));
        }

        [Test]
        public async Task TestComparePrivilegeWithValidUsers()
        {
            await using var context = new ReservationContext();
            Assert.IsFalse(await _booking.ComparePrivilege(await context.Users.FirstAsync(),
                await context.Users.OrderBy(x => x.Username).LastAsync()));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            using var context = new ReservationContext();
            var dummyData = context.Reservations.Where(x => x.StartTime > new DateTime(2021, 12, 31));
            foreach (var reservation in dummyData)
            {
                context.Reservations.Remove(reservation);
            }

            context.SaveChanges();
        }
    }
}