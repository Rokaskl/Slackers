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
    [Route("Tracker")]
    [ApiController]
    public class TimeTrackerController : ControllerBase
    {
        private IMapper _mapper;
        private IRoomService _roomService;
        private IUserService _userService;

        public TimeTrackerController(IRoomService roomService, IMapper mapper, IUserService userService)
        {
            _roomService = roomService;
            _mapper = mapper;
            _userService = userService;
        }

        [AllowAnonymous]//Pratestuoti
        [Route("time/{from}/{to}/{RoomId:int}/{UserId:int}")]
        public IActionResult GetTime(string from, string to, int RoomId, int UserId)
        {
            DateTime dateFrom = DateTime.Parse(from);
            DateTime dateTo = DateTime.Parse(to);
            int AdminId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            //App.Inst.tempRooms.FirstOrDefault(x => x.roomId == RoomId);
            if(_roomService.GetAllRooms().FirstOrDefault(x => x.roomId == RoomId).roomAdminId == AdminId)
            {
                return Ok(new
                {
                    Time = new TimeSpan(5, 20, 30)
                });
            }
            else
            {
                return Content("Failure!");
            }
        }
    }
}