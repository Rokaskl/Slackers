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
        TimeSpan GetTimeMarkDay(int roomId, int userId, int year, int month, int day);
        TimeSpan GetTimeMarkWeek(int roomId, int userId, int year, int week);
        TimeSpan GetTimeMarkMonth(int roomId, int userId, int year, int month);
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

        public TimeSpan GetTimeMarkDay(int roomId, int userId, int year, int month, int day)
        {
            //0 - Stop(false); 1 - Start(true).
            //if (!(markArray[i].Time > dateTo && markArray[i + 1].Time < dateFrom))//reikia tikrinti sita pora
            // if (markArray[i].Action == true && markArray[i + 1].Action == false)
            TimeSpan span = new TimeSpan();
            DateTime date = new DateTime(year, month, day);
            DateTime nextdaydate = date.AddDays(1);
            DateTime from;
            DateTime to;
            List<TimeMark> marks = new List<TimeMark>();
            marks = _context.TimeMarks.Where(x => x.RoomId == roomId && x.UserId == userId).ToList();
            for (int i = 0; i < marks.Count; i+=2)
            {
                from = new DateTime();
                to = new DateTime();
                if((marks[i].Time < nextdaydate && marks.Count == i + 1) || (marks[i].Time < nextdaydate && marks[i + 1].Time > date))
                //if (!(marks[i].Time/*Start*/ > nextdaydate || !( !(marks.Count < i + 1) || marks[i + 1].Time/*Stop*/ < date)))
                {
                    
                    if (marks[i].Action == true)
                    {
                        if (marks[i].Time < date)//Start buvo diena pries ieskoma
                        {
                            from = date;
                        }
                        else
                        {
                            from = marks[i].Time;
                        }

                        if (marks.Count == i + 1)//Useris siuo metu trackina laika, nes nera sustabdymo iraso.
                        {
                            if (nextdaydate > DateTime.Now)//Stop nebuvo
                            {
                                to = DateTime.Now;
                            }
                            else
                            {
                                to = nextdaydate;
                            }
                        }
                        else
                        {
                            if (marks[i + 1].Action == false)//Stop buvo
                            {
                                if (marks[i + 1].Time > nextdaydate)//Stop buvo sekancia diena nei ieskoma
                                {
                                    to = nextdaydate;
                                }
                                else
                                {
                                    to = marks[i + 1].Time;
                                }
                            }
                        }
                    }
                }
                span = span.Add(to - from);
            }
            return span;
        }

        public TimeSpan GetTimeMarkWeek(int roomId, int userId, int year, int week)
        {
            return new TimeSpan();//for now
        }

        public TimeSpan GetTimeMarkMonth(int roomId, int userId, int year, int month)
        {
            TimeSpan span = new TimeSpan();
            for (int i = 0; i < DateTime.DaysInMonth(year, month); i++)
            {
                span.Add(GetTimeMarkDay(roomId, userId, year, month, i));
            }
            return span;
        }       
    }
}
