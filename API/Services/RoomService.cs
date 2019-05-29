using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IRoomService
    {
        RoomDto GetById(int id,int requesterId);
        RoomDto GetRoom(int id);
        Room Create(Room room,List<int> users);
        void Delete(int id,int requesterId);
        IEnumerable<RoomDto> GetAllRooms();
        Room JoinGroup(int id,string uid);
        List<RoomDto> GetRoomsUsers(List<int> ids,string idString);
        List<RoomDto> GetRoomsAdmin(string idStrings);
        void KickUser(int roomId,int userId,int roomAdminId);
        bool Logout_from_room(Room room, int userId);
        Guid? UpdateGuid(int roomId, int userId);
        void LeaveRoom(int user,int roomId);
    }
    public class RoomService : IRoomService
    {
        private DataContext _context;

        public RoomService(DataContext context)
        {
            _context = context;
        }

        public Room Create(Room room,List<int> users)
        {
            if (_context.Rooms.Any(x => x.roomName == room.roomName))//valiidacijja
                throw new AppException("Room name \"" + room.roomName + "\" is already taken");
            Guid g;
            g = Guid.NewGuid();

            byte[] _users = ConvertToBytes(users);

            room.usersBytes = _users;
            room.guid = g.ToString();

            _context.Rooms.Add(room);
            _context.SaveChanges();

            return _context.Rooms.First(x=>x.roomName==room.roomName&&x.roomAdminId==room.roomAdminId);
        }
        public Room JoinGroup(int id,string guid)
        {
            var room = _context.Rooms.SingleOrDefault(x => x.guid == guid);
            var user = _context.Users.Find(id);
            if (room==null||user==null)
            {
                return null;
            }
            if (room.roomAdminId==id)
            {
                return room;
            }
            var _room = AddUser(room,user,id);
            if (_room==null)
            {
                return room;
            }
            _context.Rooms.Update(_room);
            _context.SaveChanges();
            return _room;
        }
        public RoomDto GetById(int id,int requesterId)
        {
            Room temp = _context.Rooms.Find(id);
            List<int> _users = ConvertToInts(temp.usersBytes);
            if (requesterId != temp.roomAdminId && !_users.Contains(requesterId))
            {
                //throw new AppException("User do not belong to room");
                return null;
            }
            return new RoomDto(temp.roomId,temp.roomAdminId,temp.roomName,_users);
        }
        
        public void Delete(int id,int requesterId)
        {
            var room = _context.Rooms.Find(id);
            if (room != null&&room.roomAdminId==requesterId)
            {
                ConvertToInts(room.usersBytes)?.ForEach(x => 
                {
                    KickUser(room.roomId, x, requesterId);
                    List<int> usersToKick = new List<int>();
                    usersToKick.Add(x);
                    App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 4, roomId = id, registered_room_users = usersToKick });
                });
                _context.Rooms.Remove(room);
                _context.SaveChanges();
                List<int> users = new List<int>();
                users.Add(requesterId);
                App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 5, roomId = id, registered_room_users = users });
            }
        }
        public void LeaveRoom(int user, int roomID)
        {
            Room room = _context.Rooms.Find(roomID);
            List<int> temp = ConvertToInts(room.usersBytes);
            if (!temp.Contains(user))
            {
                throw new AppException("User do not belong to room");
            }
            temp.Remove(user);
            room.usersBytes  = ConvertToBytes(temp);
            _context.Rooms.Update(room);
            _context.SaveChanges();            
            Logout_from_room(room, user);
        }
        public void KickUser(int roomId, int userId, int roomAdminId)
        {
            Room room = _context.Rooms.Find(roomId);
            if (room.roomAdminId!=roomAdminId)
            {
                throw new AppException("Not admin of this room");
            }
            List<int> temp = ConvertToInts(room.usersBytes);
            if (!temp.Contains(userId))
            {
                throw new AppException("User do not belong to room");
            }
            temp.Remove(userId);
            room.usersBytes  = ConvertToBytes(temp);
            _context.Rooms.Update(room);
            _context.SaveChanges();
            Logout_from_room(room, userId);
        }
        public IEnumerable<RoomDto> GetAllRooms()
        {
            var rooms = _context.Rooms;
            List<RoomDto> _roomDtos = new List<RoomDto>();
            foreach (Room item in rooms)
            {
                List<int> _users = ConvertToInts(item.usersBytes);               
                _roomDtos.Add(new RoomDto(item.roomId,item.roomAdminId,item.roomName,item.guid,_users));
            }
            return _roomDtos;
        }

        public List<RoomDto> GetRoomsUsers(List<int> ids,string idString)
        {
            if (ids ==null)
            {
                return null;
            }
            int id = Convert.ToInt32(idString);
            List<RoomDto> rooms = new List<RoomDto>();
            foreach (var item in ids)
            {
                var room = _context.Rooms.Find(item);
                if (room!=null)
                {
                    List<int> users = ConvertToInts(room.usersBytes);
                    if (users!=null&&users.Contains(id))
                    {
                    rooms.Add(new RoomDto(room.roomId,room.roomAdminId,room.roomName,users));
                    }
                }
            }
            return rooms;
        }
        public List<RoomDto> GetRoomsAdmin(string idString)
        {
            int id = Convert.ToInt32(idString);
            List<RoomDto> _rooms = new List<RoomDto>();
            var rooms = _context.Rooms.Where(x => x.roomAdminId == id);
            foreach (var item in rooms)
            {
                List<int> users = ConvertToInts(item.usersBytes);
                _rooms.Add(new RoomDto(item.roomId,item.roomAdminId,item.roomName,item.guid,users));                
            }
            return _rooms;
        }     
        

        //private helpers
        private byte[] ConvertToBytes(List<int> users)
        {
            if (users==null)
            {
                return null;
            }
            byte[] _users = new byte[users.Count*4];
            for (int i = 0; i < users.Count; i++)
            {
                BitConverter.GetBytes(users[i]).CopyTo(_users,i*4);
            }
            return _users;
        }

        private List<int> ConvertToInts(byte[] users)
        {
            if (users==null)
            {
                return null;
            }
            List<int> _users = new List<int>();
            for (int i = 0; i < users.Length/4; i++)
            {
                _users.Add(BitConverter.ToInt32(users,i*4));
            }
            return _users;
        }

        private Room AddUser(Room room,User user, int id)
        {
            List<int> users = ConvertToInts(room.usersBytes);//rooms users
            List<int> rooms = ConvertToInts(user.roomsBytes);//users rooms

            if (rooms==null)
            {
                rooms = new List<int>();
            }
            if (!rooms.Contains(room.roomId))
            {
                rooms.Add(room.roomId);//that confusing
                user.roomsBytes = ConvertToBytes(rooms);                
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            if (users==null)
            {
                users = new List<int>();
            }
            if (users.Contains(id))
            {
                return null;
            }

            users.Add(id);
            room.usersBytes = ConvertToBytes(users);
            return room;
        }

        public RoomDto GetRoom(int id)
        {
            Room temp = _context.Rooms.Find(id);
            List<int> _users = ConvertToInts(temp.usersBytes);
            return new RoomDto(temp.roomId, temp.roomAdminId, temp.roomName, _users);
        }

        public bool Logout_from_room(Room room, int UserId)
        {
            TempRoom tempRoom;
            if ((tempRoom = App.Inst.tempRooms.Where(x => x.usersById.Keys.Contains(UserId)).FirstOrDefault()) == null)
            {
                return true;
            }
            else
            {
                tempRoom.usersById.Remove(UserId);
                if (tempRoom.usersById.Count == 0)
                {
                    App.Inst.tempRooms.Remove(tempRoom);
                }
            }
            List<int> registeredUsers = new List<int>();
            if (ConvertToInts(room.usersBytes) != null)
                registeredUsers.AddRange(ConvertToInts(room.usersBytes));
            registeredUsers.Add(room.roomAdminId);
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 2, roomId = room.roomId, registered_room_users = registeredUsers });
            return true;
        }

        public Guid? UpdateGuid(int roomId, int userId)
        {
            Room temp = _context.Rooms.Find(roomId);
            if (temp.roomAdminId == userId)
            {
                Guid new_guid = Guid.NewGuid();
                temp.guid = new_guid.ToString();
                _context.Rooms.Update(temp);
                _context.SaveChanges();
                return new_guid;
            }
            else
            {
                return null;
            }
        }
    }
}
