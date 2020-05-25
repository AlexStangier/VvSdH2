using Application;
using Core;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApplicationTests
{
    public class ControllerTests
    {
        [Test]
        public async Task TestGetFloor()
        {   
            var rc = new RoomController();
            var rooms = await rc.getFloor(1,"A");

            Assert.AreEqual(10, rooms.Count);
        }

        [Test]
        public async Task TestFilter()
        {
            var rc = new RoomController();

            var rooms = await rc.getFloor(1,"A");
            var filteredRooms = await rc.filter(rooms, 50, new Core.Attribute());

            Assert.AreEqual(3, filteredRooms.Count);
        }

        [Test]
        public async Task TestLoginSuccess()
        {
            var uc = new UserController();

            var user = new User
            {
                Username = "udo@hs-offenburg.de",
                Password = "34cx324"
            };
            var success = await uc.login(DateTime.Now, user);

            // User exists, PW is right
            Assert.True(success);
        }

        [Test]
        public async Task TestLoginWrongPW()
        {
            var uc = new UserController();

            var user = new User
            {
                Username = "udo@hs-offenburg.de",
                Password = "12345"
            };
            var success = await uc.login(DateTime.Now, user);

            // User exists, PW is wrong
            Assert.False(success);
        }

        [Test]
        public async Task TestLoginNoUser()
        {
            var uc = new UserController();

            var user = new User
            {
                Username = "odo@hs-offenburg.de",
                Password = "34cx324"
            };
            var success = await uc.login(DateTime.Now, user);

            // User does not exist
            Assert.False(success);
        }
    }
}