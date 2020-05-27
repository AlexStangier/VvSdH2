using System;
using System.Linq;
using Application;
using ApplicationShared;
using Core;
using Microsoft.EntityFrameworkCore;

namespace VvSdHCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var context = new ReservationContext();
            var seeder = new DBSeed();
            //seeder.SeedDataToDB();
            IUser _user = new UserController();
            IBooking _booking = new BookingController();
            IRoom _room = new RoomController();

            var x = _room.GetFloor(1, "A");
            foreach (var s in x.Result)
            {
                Console.WriteLine(s.RoomNr);
            }
        }
    }
}