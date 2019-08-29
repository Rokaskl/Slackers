using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        private TcpClient client;

        public UserInfo(int id, int time)
        {
            this.id = id;
            this.time = time;
            client = null;
        }

        public TcpClient Client
        {
            get => client;
            set => client = value;
        }
    }

    public class Inst
    {
        private event RoomChangeEventHandler RoomChanged;
        private delegate void RoomChangeEventHandler(object sender, ChangeEventArgs e);
        public virtual void RaiseRoomchangedEvent(object sender, ChangeEventArgs e)
        {
            if (RoomChanged != null)
            {
                RoomChanged(sender, e);
            }
        }

        private event FriendsChangeEventHandler FriendsChanged;
        private delegate void FriendsChangeEventHandler(object sender, FriendsChangeEventArgs e);
        public virtual void RaiseFriendschangedEvent(object sender, FriendsChangeEventArgs e)
        {
            if (FriendsChanged != null)
            {
                FriendsChanged(sender, e);
            }
        }

        //Separate event is needed to delay tcp signal in order to avoid tcp signal interferation.
        private event LogEventHandler LogCreated;
        private delegate void LogEventHandler(object sender, FriendsChangeEventArgs e);
        public virtual void RaiseLogCreatedEvent(object sender, FriendsChangeEventArgs e)
        {
            if (LogCreated != null)
            {
                LogCreated(sender, e);
            }
        }

        public List<TempRoom> tempRooms;
        //private Dictionary<int, Tuple<int, int>> users;//UserId = 1, timespan value = 2, RoomId = 3
        public List<UserInfo> users;
        public TcpServer Server;
        public Dictionary<DateTime, int> User_TimeOut_List;
        public List<int> OnlineStatusUsers;

        
        public Inst()
        {
            RoomChanged += Inst_RoomChanged;
            FriendsChanged += Inst_FriendsChanged;
            LogCreated += Inst_LogCreated;
            tempRooms = new List<TempRoom>();
            User_TimeOut_List = new Dictionary<DateTime, int>();
            OnlineStatusUsers = new List<int>();
            (new TimeOutControl()).Start();
            //users = new Dictionary<int, Tuple<int, int>>();
            users = new List<UserInfo>();
            Server = new TcpServer();
            //loggedin = new List<int>();
        }

        private async void Inst_LogCreated(object sender, FriendsChangeEventArgs e)
        {
            await Task.Delay(10);

            if (e.receivers != null)
            {
                foreach (int user in e.receivers)
                {
                    this.Server.SendInfo(user, e.change.ToString(), e.data, e.senderId);
                }
            }
        }

        private void Inst_RoomChanged(object sender, ChangeEventArgs e)
        {
            //TempRoom changedRoom = tempRooms.Find(x => x.roomId == e.roomId);

            //if (changedRoom != null)
            //{
            //    foreach (KeyValuePair<int, string> user in changedRoom.usersById)
            //    {
            //        this.Server.SendInfo(user.Key, e.change.ToString());
            //    }
            //}
            if (e.registered_room_users != null)
            {
                foreach (int user in e.registered_room_users)
                {
                    this.Server.SendInfo(user, e.change.ToString(), e.data, e.senderId);
                }
            }
        }

        private void Inst_FriendsChanged(object sender, FriendsChangeEventArgs e)
        {
            if (e.receivers != null)
            {
                foreach (int user in e.receivers)
                {
                    this.Server.SendInfo(user, e.change.ToString(), e.data, e.senderId);
                }
            }
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

    public class ChangeEventArgs : EventArgs
    {
        public int change;
        public int roomId;
        public List<int> registered_room_users;
        public string data;
        public int senderId;
    }

    //Also used for LogCreated event.
    public class FriendsChangeEventArgs : EventArgs
    {
        public int change;
        public string data;
        public int senderId;
        public List<int> receivers;
    }
}
