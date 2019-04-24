using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class TimeMark
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Action { get; set; }
        public int RoomId { get; set; }
        public DateTime Time { get; set; }
    }
}
