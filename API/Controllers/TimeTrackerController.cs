using System;
using System.Collections.Generic;
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

        [AllowAnonymous]//Pratestuoti
        [Route("time/{from}/{to}/{RoomId:int}/{UserId:int}")]
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

        [Route("mark/{roomId:int}/{ac:int}")]
        public IActionResult MarkTime(int roomId, int ac)//0 - Stop(false); 1 - Start(true).
        {
            int userId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            if (ac == 1 || ac == 0)
            {
                _timeMarkService.Create(new TimeMark { UserId = userId, Action = ac == 0 ? false : true, RoomId = roomId, Time = DateTime.Now });
                _timeMarkService.Create(new TimeMark { UserId = userId, Action = ac == 0 ? false : true, RoomId = roomId, Time = DateTime.Now.AddDays(1) });//for testing purposes
            }

            return Ok();
        }
    }
}