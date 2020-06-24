using ApplicationShared;
using Core;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Application
{
    public sealed class MailController : IMail
    {
        private const string mailName = "VvSdH@web.de";
        private const string mailPW = "123d€fgh";
        private const string mailServer = "smtp.web.de";
        private const int port = 587;

        public async Task<bool> SendConfirmationMail(Reservation reservation)
        {
            return await SendMail(reservation.User.Username,
                           "Buchung erfolgreich!",
                           $"Raum {reservation.Room.RoomNr} in Gebäude {reservation.Room.Building} wurde " +
                           $"erfolgreich reserviert. Vielen Dank, dass Sie VvSdH nutzen!"); ;
        } 

        public async Task<bool> SendOverbookingMail(Reservation overbookedReservation)
        {
            return await SendMail(overbookedReservation.User.Username,
                           "Eine Ihrer Reservierungen wurde überbucht!",
                           $"Bitte beachten Sie, dass ihre Reservierung von Raum {overbookedReservation.Room.RoomNr} " +
                           $"in Gebäude {overbookedReservation.Room.Building} überbucht wurde. " +
                           $"Nutzen sie VvSdH, um einen neuen Raum zu buchen!");
        }

        private const bool SendAllMailsToOneAddress = true;

        public async Task<bool> SendMail(string toAddress, string subject, string message)
        {
            try
            {
                if(SendAllMailsToOneAddress)
                {
                    message = $"Mail wäre an {toAddress} gesendet worden.\n\n{message}";
                    toAddress = "VvSdH2@web.de";
                }

                var smtp = new SmtpClient(mailServer, port);
                smtp.Credentials = new NetworkCredential(mailName, mailPW);
                smtp.EnableSsl = true;

                var mail = new MailMessage(mailName, toAddress, subject, message);
                await smtp.SendMailAsync(mail);
                return true;
            }
            catch(SmtpFailedRecipientException)
            {
                // This exception can occur, because the server thinks this is Spam
                return true;
            }
            catch(Exception)
            {
                // User does not exist
                // Invalid adress, etc
                return false;
            }
        }
    }
}
