using ApplicationShared;
using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;

namespace ApplicationTests
{
    public class ControllerTests
    {
        private IUser _user;
        private IBooking _booking;
        private IRoom _room;

        [SetUp]
        public void SetUp()
        {
            _user = new UserController();
            _booking = new BookingController();
            _room = new RoomController();
            
            using var context = new ReservationContext();

            var dummyUser = new User
            {
                Username = "IShouldNotBeInHere",
                Password = "123",
                Reservations = new List<Reservation>(),
                Rights = context.Rights.FirstOrDefault(x => x.PrivilegeLevel == 4)
            };
        }

        [Test]
        public async Task ReturnWholeFloor()
        {
            var rooms = await _room.GetFloor(1, "A");

            Assert.AreEqual(10, rooms.Count);
        }

        [Test]
        public async Task FilterRoomsBySize()
        {
            var rooms = await _room.GetFloor(1, "A");
            var filteredRooms = await _room.Filter(rooms, 50, new Core.Attribute());

            Assert.AreEqual(3, filteredRooms.Count);
        }

        [Test]
        public async Task StandartLogin()
        {
            var user = new User
            {
                Username = "udo@hs-offenburg.de",
                Password = "34cx324"
            };
            var result = await _user.Login(DateTime.Now, user);

            // User exists, PW is correct
            Assert.True(result);
        }

        [Test]
        public async Task InvalidLoginWrongPassword()
        {
            var user = new User
            {
                Username = "udo@hs-offenburg.de",
                Password = "12345"
            };
            var result = await _user.Login(DateTime.Now, user);

            // User exists, PW is wrong
            Assert.False(result);
        }

        [Test]
        public async Task LoginWithoutCorrespondingUser()
        {
            var user = new User
            {
                Username = "odo@hs-offenburg.de",
                Password = "34cx324"
            };
            var result = await _user.Login(DateTime.Now, user);

            // User does not exist
            Assert.False(result);
        }
    }
}