using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Right
    {
        [Key]
        public string RightsName { get; set; }
        public int PrivilegeLevel { get; set; }
        public List<User> UserHasRight { get; set; }
    }
} 