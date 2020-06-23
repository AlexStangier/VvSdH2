using Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationShared
{
    public interface IMail
    {
        Task<bool> SendConfirmationMail(Reservation reservation);
        Task<bool> SendOverbookingMail(Reservation overbookedReservation);
    }
}
