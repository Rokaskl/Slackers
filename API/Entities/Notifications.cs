using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class Notifications
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public byte[] Notifications_Array { get; set; }
    }
}
