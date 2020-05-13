using System;
using Application;
using ApplicationShared;
using Core;

namespace VvSdHCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var x = new ReservationContext();
            var seeder = new DBSeed();
            seeder.SeedDataToDB();
        }
    }
}