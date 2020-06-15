using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class User : IEquatable<User>
    {
        [Key] public string Username { get; set; }
        public string Password { get; set; }
        public Right Rights { get; set; }
        public bool HasCurrentSession { get; set; }
        public List<Reservation> Reservations { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User other)
        {
            return other != null &&
                   Username == other.Username;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username);
        }
    }
}