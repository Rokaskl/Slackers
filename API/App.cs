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

        //public static TimeSpan GetTimeOfDay(DateTime day)
        //{

        //}
        
    }

    public class UserInfo
    {
        public int id;
        public int time;

        public UserInfo(int id, int time)
        {
            this.id = id;
            this.time = time;
        }
    }

    public class Inst
    {
        public List<TempRoom> tempRooms;
        //private Dictionary<int, Tuple<int, int>> users;//UserId = 1, timespan value = 2, RoomId = 3
        public List<UserInfo> users;
        //public List<int> loggedin;

        
        public Inst()
        {
            tempRooms = new List<TempRoom>();
            (new TimeOutControl()).Start();
            //users = new Dictionary<int, Tuple<int, int>>();
            users = new List<UserInfo>();
            //loggedin = new List<int>();
        }

        public bool Add(int id)
        {
            //users.Add(id, new Tuple<int, int>(0, RoomId));
            //users.Add(id, new UserInfo { time = 0, RoomId = RoomId });
            users.Add(new UserInfo(id, 0));
            return true;
        }

        public bool Remove(int id)
        {
            UserInfo user = users.FirstOrDefault(x => x.id == id);
            if (user != null)
            {
                users.Remove(user);
            }
            ///loggedin.Remove(user.id);
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
