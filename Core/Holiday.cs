using System;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Holiday
    {
        [Key]
        public DateTime Day { get; set; }
        public string Description { get; set; }
    }
}