using System;
using System.Threading.Tasks;
using Application;
using ApplicationShared;
using Core;
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


        public ControllerTests()
        {
            SetUp();
        }

        [SetUp]
        public void SetUp()
        {
            _user = new UserController();
            _booking = new BookingController();
            _room = new RoomController();
            
            dummyRoom = new Room();
            dummyUser = new User();
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
            Assert.True(result);
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
            Assert.False(result);
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
            Assert.False(result);
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
        public async Task TryCreateStandartReservation()
        {
            await using var context = new ReservationContext();
            var time = DateTime.Now;
            if (time.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                time = time.AddDays(8);
            }
            Assert.True(await _booking.CreateReservation(await context.Rooms.FindAsync(1), time, 90,await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        [Test]
        public async Task TryCreateReservationWithInvalidRoom()
        {
            var time = DateTime.Now;
            if (time.DayOfWeek == DayOfWeek.Sunday) time.AddDays(1);
            Assert.False(await _booking.CreateReservation(null, time, 90, dummyUser));
        }

        [Test]
        public async Task TryCreateReservationWithInvalidUser()
        {
            var time = DateTime.Now;
            if (time.DayOfWeek == DayOfWeek.Sunday) time.AddDays(1);
            Assert.False(await _booking.CreateReservation(dummyRoom, time, 90, null));
        }

        [Test]
        public async Task TryCreateReservationWithNegativeDuration()
        {
            await using var context = new ReservationContext();
            var time = DateTime.Now;
            if (time.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                time = time.AddDays(8);
            }
            Assert.True(await _booking.CreateReservation(await context.Rooms.FindAsync(1),time, -90,await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }
        
        /*
        [Test]
        public async Task TryCreateReservationWithOuterlimitDuration()
        {
            SetUp();
            await using var context = new ReservationContext();
            Assert.True(await _booking.CreateReservation(await context.Rooms.FindAsync(1),DateTime.Now, 99999999,await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }
        */
        
        [Test]
        public async Task TryCreateRerservation()
        {
            await using var context = new ReservationContext();
            var time = DateTime.Now;
            if (time.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                time = time.AddDays(8);
            }
            Assert.True(await _booking.CreateReservation(await context.Rooms.FindAsync(1),time, 90,await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        [Test]
        public async Task TryOverbookReservation()
        {
            await using var context = new ReservationContext();
            Assert.True(await _booking.CreateReservation(await context.Rooms.FindAsync(1), DateTime.Now.AddHours(25), 90, await context.Users.FindAsync("udo@hs-offenburg.de")));
        }

        [Test]
        public async Task TrySundayRerservation()
        {
            await using var context = new ReservationContext();
            Assert.False(await _booking.CreateReservation(await context.Rooms.FindAsync(1), new DateTime(2020, 8, 30, 12, 0, 0), 90, await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        [Test]
        public async Task TryHolidayRerservation()
        {
            await using var context = new ReservationContext();
            Assert.False(await _booking.CreateReservation(await context.Rooms.FindAsync(1), new DateTime(2020, 12, 26, 12, 0, 0), 90, await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }
        
        [Test]
        public async Task GetReservationsForSpecifiedUser()
        {
            await using var context = new ReservationContext();
            Assert.NotNull(_booking.GetUserReservations(await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }
        
        [Test]
        public async Task GetReservationsForInvalidUser()
        {
            await using var context = new ReservationContext();
            var result = await _booking.GetUserReservations(dummyUser);
            Assert.GreaterOrEqual(1,result?.Count ?? 0);
            
        }
    }
}