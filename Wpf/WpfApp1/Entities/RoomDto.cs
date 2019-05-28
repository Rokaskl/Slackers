using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    /// <summary>
    /// Room data class
    /// </summary>
    public class RoomDto
    {
        public int roomId { get; set; }
        public int roomAdminId { get; set; }
        public string roomName { get; set; }
        public string guid {get;set;}
        public int[] users { get; set; }

        public RoomDto(int roomId, int roomAdminId, string roomName,int[] ids,string guid)
        {
            this.roomId = roomId;
            this.roomAdminId = roomAdminId;
            this.roomName = roomName;
            this.users =ids;
            this.guid = guid;
        }

        public RoomDto()
        {
        }
    }

}
