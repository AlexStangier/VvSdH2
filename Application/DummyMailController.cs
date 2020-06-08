using ApplicationShared;
using Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    /// <summary>
    /// This class provides a dummy implementation of the IMail Interface.
    /// It is useful for testing purposes, if you don't want to send emails every time.
    /// </summary>
    public sealed class DummyMailController : IMail
    {
        public async Task SendConfirmationMail(Reservation reservation)
        {
        }

        public async Task SendOverbookingMail(Reservation overbookedReservation)
        {
        }
    }
}
