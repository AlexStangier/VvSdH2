using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class Room
    {
        public int RoomId { get; set; }
        public int RoomNr { get; set; }
        public string Building { get; set; }
        public int Size { get; set; }
        public int Floor { get; set; }
        public Attribute Attribute { get; set; }
    }
}