using ApplicationShared;
using Core;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Application;

namespace ApplicationTests
{
    public class ControllerTests
    {
        private IUser _user;
        private IBooking _booking;
        private IRoom _room;
        private User dummyUser;
        private Room dummyRoom;


        [SetUp]
        public void SetUp()
        {
            _user = new UserController();
            _booking = new BookingController();
            _room = new RoomController();

            dummyRoom = new Room
            {
                Building = "A",
                Floor = 1,
                RoomNr = 100,
                Size = 30
            };
        }

        [Test]
        public async Task ReturnWholeFloor()
        {
            SetUp();
            var rooms = await _room.GetFloor(1, "A");

            Assert.AreEqual(10, rooms.Count);
        }

        [Test]
        public async Task FilterRoomsBySize()
        {
            SetUp();
            var rooms = await _room.GetFloor(1, "A");
            var filteredRooms = await _room.Filter(rooms, 50, new Core.Attribute());

            Assert.AreEqual(3, filteredRooms.Count);
        }

        [Test]
        public async Task StandartLogin()
        {
            SetUp();
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
        public async Task InvalidLoginWrongPassword()
        {
            SetUp();
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
        public async Task LoginWithoutCorrespondingUser()
        {
            SetUp();
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
        public async Task StandartLogout()
        {
            SetUp();
            Assert.True(await _user.Logout("udo@hs-offenburg.de"));
        }

        [Test]
        public async Task LogoutWithInvalidCredentials()
        {
            SetUp();
            Assert.False(await _user.Logout(""));
        }

        [Test]
        public async Task GetRoomByBuildingAndNumber()
        {
            SetUp();
            Assert.NotNull(await _room.GetCurrentStatusForRoom(100, "A"));
        }

        [Test]
        public async Task GetRoomWithInvalidRoomNumber()
        {
            SetUp();
            Assert.Null(await _room.GetCurrentStatusForRoom(0, "A"));
        }

        [Test]
        public async Task GetRoomWithInvalidBuilding()
        {
            SetUp();
            Assert.Null(await _room.GetCurrentStatusForRoom(100, ""));
        }

        [Test]
        public async Task GetFloorByBuildingAndNumber()
        {
            SetUp();
            Assert.NotNull(await _room.GetCurrentStatusForFloor("A", 1));
        }

        [Test]
        public void GetFloorWithInvalidRoomNumber()
        {
            SetUp();
            Assert.AreEqual(0, _room.GetCurrentStatusForFloor("A", 0).Result.Count);
        }

        [Test]
        public void GetFloorWithInvalidBuilding()
        {
            SetUp();
            Assert.AreEqual(0, _room.GetCurrentStatusForFloor("", 1).Result.Count);
        }

        [Test]
        public async Task CreateStandartReservation()
        {
            SetUp();
            await using var context = new ReservationContext();
            Assert.True(await _booking.CreateReservation(await context.Rooms.FindAsync(1), DateTime.Now, 90,await context.Users.FindAsync("alex@stud.hs-offenburg.de")));
        }

        [Test]
        public async Task CreateReservationWithInvalidRoom()
        {
            SetUp();
            Assert.False(await _booking.CreateReservation(null, DateTime.Now, 90, dummyUser));
        }

        [Test]
        public async Task CreateReservationWithInvalidUser()
        {
            SetUp();
            Assert.False(await _booking.CreateReservation(dummyRoom, DateTime.Now, 90, null));
        }
    }
}