using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Holyday
    {
        [Key]
        public DateTime day { get; set; }
        public string reason { get; set; }
    }
}