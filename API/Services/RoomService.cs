using System;
using System.Collections.Generic;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IRoomService
    {
        RoomDto GetById(int id);
        Room Create(Room room,int[] users);
        void Delete(int id);
        IEnumerable<RoomDto> GetAllRooms();
    }
    public class RoomService : IRoomService
    {
        private DataContext _context;

        public RoomService(DataContext context)
        {
            _context = context;
        }

        public Room Create(Room room,int[] users)
        {
            byte[] _users = ConvertToBytes(users);
            room.usersByte = _users;
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return room;
        }

        public RoomDto GetById(int id)
        {
            Room temp = _context.Rooms.Find(id);
            int[] _users = ConvertToInts(temp.usersByte);
            return new RoomDto(temp.roomId,temp.roomAdminId,temp.roomName,_users);
        }
        
        public void Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room != null)
            {
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            }
        }
        private byte[] ConvertToBytes(int[] users)
        {
            byte[] _users = new byte[users.Length*4];
            for (int i = 0; i < users.Length; i++)
            {
                BitConverter.GetBytes(users[i]).CopyTo(_users,i*4);
            }
            return _users;
        }
        private int[] ConvertToInts(byte[] users)
        {
            int[] _users = new int[users.Length/4];
            for (int i = 0; i < users.Length/4; i++)
            {
                _users[i] = BitConverter.ToInt32(users,i*4);
            }
            return _users;
        }

        public IEnumerable<RoomDto> GetAllRooms()
        {
            var rooms = _context.Rooms;
            List<RoomDto> _roomDtos = new List<RoomDto>();
            foreach (Room item in rooms)
            {
                int[] _users = ConvertToInts(item.usersByte);
                _roomDtos.Add(new RoomDto(item.roomId,item.roomAdminId,item.roomName,_users));
            }
            return _roomDtos;
        }
    }
}
