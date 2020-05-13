using System;

namespace Core
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        
        public DateTime Start { get; set; }
        
        public DateTime End { get; set; }
        
        public User User { get; set; }
        
        public Room Room { get; set; }
    }
}