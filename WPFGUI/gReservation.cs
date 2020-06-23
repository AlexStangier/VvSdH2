using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGUI
{
    class gReservation
    {
        public static Reservation reservation { get; set; }
        public static List<Reservation> reservations { get; set; }

        public gReservation(Reservation res, List<Reservation> ress)
        {
            reservation = new Reservation();
            reservation = res;
            reservations = ress;
        }
    }
}
