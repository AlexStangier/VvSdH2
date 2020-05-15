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

            var floor1 = context.Rooms.Where(x => x.Floor == 1).Include(y => y.Attribute).ToList();
            foreach (var el in floor1)
            {
                Console.WriteLine($"{el.RoomNr}    {el.Size}    {el.Attribute.AirCon}    {el.Attribute.Computers}");
            }

            /**context.Reservations.Add(new Reservation
            {
                Start = new DateTime(2020,5,13,15,0,0),
                End = new DateTime(2020,5,13,16,30,0),
                Room = context.Rooms.FirstOrDefault(x => x.RoomNr == 100),
                User = context.Users.FirstOrDefault(x => x.Username.Equals("Peter"))
            });

            context.SaveChanges();**/
        }
    }
}