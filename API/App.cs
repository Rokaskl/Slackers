using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Dtos;

namespace WebApi
{
    /// <summary>
    /// Programos instance
    /// </summary>
    public static class App
    {
        public static Inst Inst;

        public static void CreateInst()
        {
            Inst = new Inst();
        }
        
    }

    public class UserInfo
    {
        public int id;
        public int time;
        public int RoomId;

        public UserInfo(int id, int time, int RoomId)
        {
            this.id = id;
            this.time = time;
            this.RoomId = RoomId;
        }
    }

    public class Inst
    {
        public List<TempRoom> tempRooms;
        //private Dictionary<int, Tuple<int, int>> users;//UserId = 1, timespan value = 2, RoomId = 3
        public List<UserInfo> users;

        
        public Inst()
        {
            tempRooms = new List<TempRoom>();
            (new TimeOutControl()).Start();
            //users = new Dictionary<int, Tuple<int, int>>();
            users = new List<UserInfo>();
        }

        public bool Add(int id, int RoomId)
        {
            //users.Add(id, new Tuple<int, int>(0, RoomId));
            //users.Add(id, new UserInfo { time = 0, RoomId = RoomId });
            users.Add(new UserInfo(id, 0, RoomId));
            return true;
        }

        public bool Remove(int id)
        {
            users.Remove(users.First(x => x.id == id));
            return true;
        }
    }

    public class TempRoom
    {
        public Dictionary<int, string> usersById;//value - userio statusas: A-Active(Green), B-Busy/Away(Yellow), C-F*ckOff(Red).  
        public List<Tuple<int, string, DateTime>> log;// room log; structure: UserId, action, time;
        public int roomId;

        public TempRoom(int roomid)
        {
            usersById = new Dictionary<int, string>();
            this.roomId = roomid;
        }
    }
}
