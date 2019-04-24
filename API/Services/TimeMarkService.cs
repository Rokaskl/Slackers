using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Dtos;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface ITimeMarkService
    {
        //RoomDto GetById(int id, int requesterId);
        void Create(TimeMark mark);
        //void Delete(int id, int requesterId);
        List<TimeMarkDto> GetAllTimeMarks();
        //Room JoinGroup(int id, string uid);
        //List<RoomDto> GetRoomsUsers(List<int> ids, string idString);
        //List<RoomDto> GetRoomsAdmin(string idStrings);
    }
    public class TimeMarkService : ITimeMarkService
    {
        private DataContext _context;

        public TimeMarkService(DataContext context)
        {
            _context = context;
        }

        public void Create(TimeMark mark)
        {
            //if (_context.Rooms.Any(x => x.roomName == room.roomName))//valiidacijja
            // throw new AppException("Room name \"" + room.roomName + "\" is already taken");

            //_context.TimeMarks

            //Guid g;
            //g = Guid.NewGuid();

            //byte[] _users = ConvertToBytes(users);

            //room.usersBytes = _users;
            //room.guid = g.ToString();

            _context.TimeMarks.Add(mark);
            _context.SaveChanges();

            //return room;
        }

        public List<TimeMarkDto> GetAllTimeMarks()
        { 
            List<TimeMarkDto> marks = new List<TimeMarkDto>();
            foreach(TimeMark mark in _context.TimeMarks)
            {
                //(int UserId, bool Action, int RoomId, DateTime Time)
                marks.Add(new TimeMarkDto(mark.UserId, mark.Action, mark.RoomId, mark.Time));
                //TimeMarkDto markDto = new TimeMarkDto();
                //markDto.Action = mark.Action;
                //markDto.RoomId = mark.RoomId;
                //markDto.Time = mark.Time;
                //markDto.UserId = mark.UserId;
                //marks.Add(markDto);
            }
            return marks;
        }
    }
}
