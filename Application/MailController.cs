using ApplicationShared;
using Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public sealed class MailController : IMail
    {
        private const string mailName = "smtp.web.de";
        private const string mailPW = "123d€fgh";
        private const string mailServer = "smtp.web.de";
        private const int port = 587;

        // TODO Maybe customize mail content with information about the booking

        public async Task SendConfirmationMail(Reservation reservation)
        {
            await SendMail(reservation.User.Username,
                           "Booking Succeeded!",
                           $"Room {reservation.Room.RoomNr} in building {reservation.Room.Building} was" +
                           $"reservated successfully. Thanks for using VvSdH!"); ;
        } 

        public async Task SendOverbookingMail(Reservation overbookedReservation)
        {
            await SendMail(overbookedReservation.User.Username,
                           "One of your reservations was overbooked",
                           $"Please be aware that your reservations of room {overbookedReservation.Room.RoomNr}" +
                           $"in building {overbookedReservation.Room.Building} has been overbooked." +
                           $"Please login to VvSdH for further information, or to book another room.");
        }

        public async Task SendMail(string toAddress, string subject, string message)
        {
            var smtp = new SmtpClient(mailServer, port);
            smtp.Credentials = new NetworkCredential(mailName, mailPW);
            smtp.EnableSsl = true;

            var mail = new MailMessage(mailName, toAddress, subject, message);
            await smtp.SendMailAsync(mail);
        }
    }
}
