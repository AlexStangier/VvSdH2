using Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationShared
{
    public interface IMail
    {
        Task SendConfirmationMail(Reservation reservation);
        Task SendOverbookingMail(Reservation overbookedReservation);
    }
}
