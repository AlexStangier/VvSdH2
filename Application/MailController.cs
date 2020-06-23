﻿using ApplicationShared;
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
        private const string mailName = "VvSdH@web.de";
        private const string mailPW = "123d€fgh";
        private const string mailServer = "smtp.web.de";
        private const int port = 587;

        public async Task<bool> SendConfirmationMail(Reservation reservation)
        {
            return await SendMail(reservation.User.Username,
                           "Booking Succeeded!",
                           $"Room {reservation.Room.RoomNr} in building {reservation.Room.Building} was " +
                           $"reservated successfully. Thanks for using VvSdH!"); ;
        } 

        public async Task<bool> SendOverbookingMail(Reservation overbookedReservation)
        {
            return await SendMail(overbookedReservation.User.Username,
                           "One of your reservations was overbooked",
                           $"Please be aware that your reservations of room {overbookedReservation.Room.RoomNr} " +
                           $"in building {overbookedReservation.Room.Building} has been overbooked. " +
                           $"Please login to VvSdH for further information, or to book another room.");
        }

        public async Task<bool> SendMail(string toAddress, string subject, string message)
        {
            try
            {
#if DEBUG
                message = $"Mail would have been send to {toAddress}\n\n{message}";
                toAddress = "VvSdH2@web.de";
#endif
                var smtp = new SmtpClient(mailServer, port);
                smtp.Credentials = new NetworkCredential(mailName, mailPW);
                smtp.EnableSsl = true;

                var mail = new MailMessage(mailName, toAddress, subject, message);
                await smtp.SendMailAsync(mail);
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
