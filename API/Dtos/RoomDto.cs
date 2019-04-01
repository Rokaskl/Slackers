using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class RoomDto
    {
        public int roomId { get; set; }
        public int roomAdminId { get; set; }
        public string roomName { get; set; }
        public string guid {get;set;}
        public List<int> users { get; set; }    

        public RoomDto(int roomId, int roomAdminId, string roomName,List<int> ids)
        {
            this.roomId = roomId;
            this.roomAdminId = roomAdminId;
            this.roomName = roomName;
            this.users =ids;
        }

        public RoomDto()
        {

        }

        public RoomDto(int roomId, int roomAdminId, string roomName, string guid, List<int> users)
        {
            this.roomId = roomId;
            this.roomAdminId = roomAdminId;
            this.roomName = roomName;
            this.guid = guid;
            this.users = users;
        }

        public RoomDto(int roomAdminId, string roomName)
        {
            this.roomAdminId = roomAdminId;
            this.roomName = roomName;
        }

        public RoomDto(string roomName)
        {
            this.roomName = roomName;
        }
    }

}
