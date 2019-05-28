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
        void ResolveTimeOuts(int userId);
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
            if (date > DateTime.Now)
            {
                return span;
            }
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

        public void ResolveTimeOuts(int userId)
        {
            Dictionary<DateTime, int> timeouts = App.Inst.User_TimeOut_List.Where(x => x.Value == userId)?.OrderBy(z => z.Key).ToDictionary(y => y.Key, y => y.Value);
            if (timeouts.Count == 0)//User had no timeouts
            {
                return;
            }
            List<TimeMark> marks = new List<TimeMark>();
            marks = _context.TimeMarks.Where(x => x.UserId == userId).OrderBy(y => y.Time).ToList();
            if (marks.Count == 0)
            {
                foreach (KeyValuePair<DateTime, int> item in timeouts)
                {
                    App.Inst.User_TimeOut_List.Remove(item.Key);
                }
                return;
            }
            for (int i = 0; i < marks.Count - 1; i+=2)
            {
                if (marks[i].Action == true && marks[i + 1].Action == true)//2 start marks in a row
                {
                    Dictionary<DateTime, int> timeout_Time = timeouts.Where(x => x.Key > marks[i].Time && x.Key < marks[i + 1].Time).ToDictionary(y => y.Key, y => y.Value);//Should always exist
                    if (timeout_Time != null)
                    {
                        _context.TimeMarks.Add(new TimeMark() { Action = false/*Stop*/, RoomId = marks[i].RoomId, UserId = userId, Time = timeout_Time.First().Key });
                        foreach (KeyValuePair<DateTime, int> item in timeout_Time)
                        {
                            App.Inst.User_TimeOut_List.Remove(item.Key);
                        }
                    }
                }
            }

            if (marks.Last().Action == true)//Last time mark was "Start"
            {
                Dictionary<DateTime, int> timeout_Time = timeouts.Where(x => x.Key > marks.Last().Time).ToDictionary(y => y.Key, y => y.Value);
                if (timeout_Time != null)
                {
                    _context.TimeMarks.Add(new TimeMark() { Action = false/*Stop*/, RoomId = marks.Last().RoomId, UserId = userId, Time = timeout_Time.First().Key });
                    foreach (KeyValuePair<DateTime, int> item in timeout_Time)
                    {
                        App.Inst.User_TimeOut_List.Remove(item.Key);
                    }
                }
            }

            _context.SaveChanges();
        }
    }
}
