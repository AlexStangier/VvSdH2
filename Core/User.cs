using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        
        public Right Rights { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }
    }
}

