using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class User
    {
        [Key] public string Username { get; set; }
        public string Password { get; set; }
        public Right Rights { get; set; }
        public bool HasCurrentSession { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}