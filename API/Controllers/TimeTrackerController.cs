using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Dtos;
using WebApi.Entities;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TimeTrackerController : ControllerBase
    {
        private IMapper _mapper;
        private IRoomService _roomService;
        private IUserService _userService;
        private ITimeMarkService _timeMarkService;

        public TimeTrackerController(IRoomService roomService, IMapper mapper, IUserService userService, ITimeMarkService timeMarkService)
        {
            _roomService = roomService;
            _mapper = mapper;
            _userService = userService;
            _timeMarkService = timeMarkService;
        }

        //[AllowAnonymous]//Pratestuoti
        //[Route("time/{from}/{to}/{RoomId:int}/{UserId:int}")]
        public IActionResult GetTime(string from, string to, int RoomId, int UserId)
        {
            try
            {
                DateTime dateFrom = DateTime.Parse(from);
                DateTime dateTo = DateTime.Parse(to);
                if (dateFrom == dateTo)
                {
                    return BadRequest("to - from = 0");
                }
                int AdminId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
                TimeSpan[] days = new TimeSpan[dateTo.Subtract(dateFrom).Days + 1];
                //App.Inst.tempRooms.FirstOrDefault(x => x.roomId == RoomId);
                if (_roomService.GetAllRooms().FirstOrDefault(x => x.roomId == RoomId)?.roomAdminId == AdminId)
                {
                    //atliekami skaiciavimai...
                    List<TimeMarkDto> info = _timeMarkService.GetAllTimeMarks();
                    TimeMarkDto[] markArray = info.Where(x => x.RoomId == RoomId && x.UserId == UserId).OrderBy(z => z.Time).ToArray();
                    for (int i = 0; i < markArray.Length; i+=2)
                    {
                        if (!(markArray[i].Time > dateTo && markArray[i + 1].Time < dateFrom))//reikia tikrinti sita pora
                        {
                            if (markArray[i].Action == true && markArray[i + 1].Action == false)//si pora gera, t.y. pradzioj start, paskui stop
                            {
                                DateTime fromD;
                                DateTime toD;
                                if (markArray[i].Time < dateFrom)
                                {
                                    //skaiciuoti tos dienos laika nuo dateFrom
                                    fromD = dateFrom;
                                }
                                else
                                {
                                    //skaiciuoti tos dienos laika nuo markArray[i].Time
                                    fromD = markArray[i].Time;
                                }
                                if (markArray[i + 1].Time > dateTo)
                                {
                                    //skaiciuoti tos dienos laika iki dateTo
                                    toD = dateTo;
                                }
                                else
                                {
                                    //skaiciuoti tos dienos laika iki markArray[i + 1].Time
                                    toD = markArray[i + 1].Time;
                                }
                                TimeSpan timeSpan = toD.Subtract(fromD);
                                days[fromD.Subtract(dateFrom).Days] = days[fromD.Subtract(dateFrom).Days] + timeSpan;
                            }
                        }
                    }
                }

                return Ok(days);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        //[AllowAnonymous]//Pratestuoti --NENAUDOJAMAS
        [Route("timeuser/{from}/{to}/{RoomId:int}/{UserId:int}/{format:int}")]
        public IActionResult GetTimeSpanUser(string from, string to, int RoomId, int UserId, int format)
        {
            DateTime datefrom = DateTime.Parse(from);
            DateTime dateto = DateTime.Parse(to);
            List<TimeSpan> spans = new List<TimeSpan>();

            try
            {
                while (datefrom < dateto)
                {
                    //span.Add(_timeMarkService.GetTimeMarkDay(RoomId, UserId, datefrom.Year, datefrom.Month, datefrom.Day));
                    datefrom.AddDays(1);
                }
            }
            catch (Exception exception)
            {

            }
            return Content("");
        }

        [Route("timeroom/{from}/{to}/{RoomId:int}/{format:int}")]
        public IActionResult GetTimeSpanRoom(string from, string to, int RoomId, int format)
        {
            DateTime datefrom = DateTime.Parse(from);
            DateTime dateto = DateTime.Parse(to);
            List<TimeSpan> spans = new List<TimeSpan>();
            RoomDto room = _roomService.GetAllRooms().FirstOrDefault(x => x.roomId == RoomId);
            if (room == null)
            {
                return BadRequest("room doesnt exist");
            }
            List<int> users = new List<int>();
            if (room.users != null)
            {
                users.AddRange(room.users.Select(x => x));
            }
            users.Add(room.roomAdminId);
            Dictionary<int, List<TimeSpan>> info = new Dictionary<int, List<TimeSpan>>();
            for (int i = 0; i < users.Count; i++)
            {
                info.Add(users[i], new List<TimeSpan>());
            }

            try
            {
                while (datefrom < dateto)
                {
                    foreach (int userId in users)
                    {
                        TimeSpan calculated = _timeMarkService.GetTimeMarkDay(RoomId, userId, datefrom.Year, datefrom.Month, datefrom.Day);
                        info[userId].Add(calculated);// = info[userId].Add(calculated);
                    }

                    datefrom = datefrom.AddDays(1);
                }

                switch (format)
                {
                    case 0:
                    {
                        Dictionary<int, int> data = new Dictionary<int, int>();
                        foreach (KeyValuePair<int, List<TimeSpan>> pair in info)
                        {
                            data.Add(pair.Key, (int) Math.Round(pair.Value.Sum(x => x.TotalMinutes), 0));
                        }

                        return Ok(data);
                    }
                    case 1:
                    {
                        List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                        foreach (KeyValuePair<int, List<TimeSpan>> pair in info)
                        {
                            Dictionary<string, string> info_dic = new Dictionary<string, string>();
                            info_dic.Add("Id", pair.Key.ToString());
                            info_dic.Add("Times",
                                string.Join(',', pair.Value.Select(x => (int) Math.Round(x.TotalMinutes, 0))));
                            data.Add(info_dic);
                        }

                        return Ok(data);
                    }
                    case 2:
                    {
                       // Random rnd = new Random();
                            //{
                            //    name: "Rokas",
                            //    type: "bar",
                            //    data: [
                            //    {
                            //        x: "02-10-2017 GMT",
                            //        y: 34
                            //    },
                            Dictionary<string, Dictionary<string, int>> data =
                            new Dictionary<string, Dictionary<string, int>>();
                        foreach (KeyValuePair<int, List<TimeSpan>> pair in info)
                        {

                                DateTime startdate = DateTime.Parse(from);
                            Dictionary<string, int> days = new Dictionary<string, int>();

                            pair.Value.ForEach(x => 
                            {
                               // int sk = rnd.Next(425);

                                days.Add(
                                    startdate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture) + " GMT",
                                    (int) Math.Round(x.TotalMinutes, 0));
                                startdate = startdate.AddDays(1);
                            });
                            data.Add(_userService.GetById(pair.Key).Username, days);
                        }

                        return Ok(data.Select(x => new { name = x.Key, type = "bar", data = x.Value.Select(z => new { x = z.Key, y = z.Value })}));
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                
            }
            return BadRequest();
        }

        [Route("mark/{roomId:int}/{ac:int}")]
        public IActionResult MarkTime(int roomId, int ac)//0 - Stop(false); 1 - Start(true).
        {
            int userId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
                // int userId = 2;
            if (ac == 2 || ac == 0)
            {
                _timeMarkService.Create(new TimeMark { UserId = userId, Action = ac == 0 ? false : true, RoomId = roomId, Time = DateTime.Now });

                //Palikta testavimo tikslams. Cia yra pati sudetingiausia situacija laiku pasiskirstymo.
                //_timeMarkService.Create(new TimeMark { UserId = userId, Action = true, RoomId = roomId, Time = new DateTime(2019, 05, 12, 12, 10, 10) });//for testing purposes
                //_timeMarkService.Create(new TimeMark { UserId = userId, Action = false, RoomId = roomId, Time = new DateTime(2019, 05, 15, 12, 10, 10) });//for testing purposes

                //_timeMarkService.Create(new TimeMark { UserId = userId, Action = true, RoomId = roomId, Time = new DateTime(2019, 05, 15, 13, 0, 0) });//for testing purposes
                //_timeMarkService.Create(new TimeMark { UserId = userId, Action = false, RoomId = roomId, Time = new DateTime(2019, 05, 15, 14, 20, 10) });//for testing purposes
                //_timeMarkService.Create(new TimeMark { UserId = userId, Action = true, RoomId = roomId, Time = new DateTime(2019, 05, 15, 15, 0, 0) });//for testing purposes
            }

            return Ok();
        }
    }
}