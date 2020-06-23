using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGUI
{
    class gReservation
    {
        public static Reservation reservation { get; set; }

        public gReservation(Reservation res)
        {
            reservation = new Reservation();
            reservation = res;
        }
    }
}
