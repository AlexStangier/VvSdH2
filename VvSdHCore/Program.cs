using System;
using System.Threading.Tasks;
using Application;
using Core;

namespace VvSdHCore
{
    class Program
    {
        static void Main(string[] args)
        {
            SetInvariantCulture();

            Seed();
            // TestMail();
        }

        /// <summary>
        /// This method is useful to force the english error messages.
        /// Makes it easier to google your errors.
        /// </summary>
        public static void SetInvariantCulture()
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.CurrentCulture = culture;
            System.Globalization.CultureInfo.CurrentUICulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        static void Seed()
        {
            Console.WriteLine("Hello World!");
            using var context = new ReservationContext();
            var seeder = new DBSeed();
            //seeder.SeedDataToDB();
        }

        static void TestMail()
        {
            var mailController = new MailController();
            
            var result = Task.Run(async () => await mailController.SendMail("ltartler@hs-offenburg.de", "Test", "Hello World"));

            result.Wait();
        }
    }
}