using System;
using Core;

namespace VvSdHCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using var context = new ReservationContext();
            var seeder = new DBSeed();
            //seeder.SeedDataToDB();
        }
    }
}