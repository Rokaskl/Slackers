using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int CreatorId { get; set; }
        public string Message { get; set; }
        public string Header { get; set; }
        public int Status { get; set; }//0 - new; 1 - in progress; 2 - done; others - reserved for further usage.
    }
}
