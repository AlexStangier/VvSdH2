using System;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Holiday
    {
        [Key]
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}