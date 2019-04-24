using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class TimeMarkDto
    {
        public int UserId;
        public bool Action;
        public int RoomId;
        public DateTime Time;

        public TimeMarkDto (int UserId, bool Action, int RoomId, DateTime Time)
        {
            this.UserId = UserId;
            this.Action = Action;
            this.RoomId = RoomId;
            this.Time = Time;
        }

        public TimeMarkDto() { }
    }
}
