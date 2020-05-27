using System;

namespace Core
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public User User { get; set; }
        
        public Room Room { get; set; }
    }
}