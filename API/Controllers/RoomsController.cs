using System;
using System.Collections.Generic;
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

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private IMapper _mapper;
        private IRoomService _roomService;

        public RoomsController(IRoomService roomService,IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
        }

        // GET: Rooms
        [HttpGet]
        public IActionResult GetRooms()//gražina visus roomus, kol testavimui reikia, paskui reiks ištrint
        {
            var _rooms = _roomService.GetAllRooms();
            return Ok(_rooms);
        }

        //// GET: Rooms/5
        //[HttpGet("{id}")]//žemiau yra kur ima pagal requesterio id>> atskirai useriams be guid ir atskirai adminams su guid
        //public IActionResult GetRoom(int id)//reiks padaryt kad gražintų tik jei useris tam rūmui priklauso
        //{
        //    var room = _roomService.GetById(id);     
        //    return Ok(room);
        //}

        // POST: Rooms/register
        [HttpPost("register")]
        public IActionResult Register([FromBody]RoomDto room)// registruoja rooma, užtenka pateikti admino ID ir roomo vardą
        {//reiks daryti validaciją kad priimtų tik su unikaliais vardais
            var roomie = _mapper.Map<Room>(room);
            try
            {
            _roomService.Create(roomie,room.users);//reiks įvesk validaciiją dėl vardų
            return Ok();//gražina kad sėkmingai registruota
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: Rooms/users_get_rooms
        [HttpGet("users_get_rooms")]
        public IActionResult UsersGetRooms(RequestRooms rooms)//negaliu tiesiog dėti list idk why
        {
            string idString = Request.HttpContext.User.Identity.Name;//ima requesterio id
            var _rooms = _roomService.GetRoomsUsers(rooms.rooms,idString);//ima tik tuos rūmus kuriuose jis registruotas
            return Ok(_rooms);//gražina roomus su roomId roomAdminId roomName users
        }

        // GET: Rooms/admin_get_rooms
        [HttpGet("admin_get_rooms")]
        public IActionResult AdminGetRooms(RequestRooms rooms)//negaliu tiesiog dėti list idk why
        {
            string idString = Request.HttpContext.User.Identity.Name;//ima requesterio id
            var _rooms = _roomService.GetRoomsAdmin(idString);//ima tik tuos rūmus kuriuose jis adminas
            return Ok(_rooms);//gražina roomus su roomId roomAdminId roomName guid users
        }

        // PUT: Rooms/join_group
        [HttpPut("join_group")]
        public IActionResult JoinGroup(JoinGroup guid)//ima tik sukurtus objektus su get set
        {
            int id = Convert.ToInt32(Request.HttpContext.User.Identity.Name);//gauna authentifikuoto asmens id, nes taip saugiau
            try//
            {
            Room temp = _roomService.JoinGroup(id,guid.guid);//įrašo į roomo userių šąrašą ir userio roomų sąrašą
                if (temp == null)
                {
                    throw new AppException( "Room not found");
                }
            return Ok();
            }
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }

        // DELETE: Rooms/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            _roomService.Delete(id,requesterId);//tik roomo adminas gali ištrinti roomą
            return Ok();
        }
    }
}
