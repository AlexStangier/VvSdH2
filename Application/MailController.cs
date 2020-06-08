using ApplicationShared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class MailController : IMail
    {
        private const string mailName = "smtp.web.de";
        private const string mailPW = "123d€fgh";
        private const string mailServer = "smtp.web.de";
        private const int port = 587;

        // TODO Maybe customize mail content with information about the booking

        public async Task SendConfirmationMail(string toAddress)
        {
            await SendMail(toAddress,
                           "Booking Succeeded!",
                           "Room was reservated successfully. Thanks for using VvSdH!");
        }

        public async Task SendOverbookingMail(string toAddress)
        {
            await SendMail(toAddress,
                           "One of your reservations was overbooked",
                           "Please be aware that one of your reservations has been overbooked. Please login to VvSdH for further information.");
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
