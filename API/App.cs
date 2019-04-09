using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public class Inst
    {
        public List<TempRoom> tempRooms;

        public Inst()
        {
            tempRooms = new List<TempRoom>();
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
