﻿using System;
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
using Newtonsoft.Json.Linq;
using System.Linq;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private IMapper _mapper;
        private IRoomService _roomService;
        private IUserService _userService;
        private ILogService _logService;

        public RoomsController(IRoomService roomService,IMapper mapper, IUserService userService, ILogService logService)
        {
            _roomService = roomService;
            _mapper = mapper;
            _userService = userService;
            _logService = logService;
        }

        // GET: Rooms
        [HttpGet]
        public IActionResult GetRooms()//gražina visus roomus, kol testavimui reikia, paskui reiks ištrint
        {
            var _rooms = _roomService.GetAllRooms();
            return Ok(_rooms);
        }

        // GET: Rooms/5
        [HttpGet("{id}")]
        public IActionResult GetRoom(int id)//reiks padaryt kad gražintų tik jei useris tam rūmui priklauso
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            var room = _roomService.GetById(id,requesterId);
            return Ok(room);
        }

        // POST: Rooms/register
        [HttpPost("register")]
        public IActionResult Register([FromBody]RoomDto room)// registruoja rooma, užtenka pateikti admino ID ir roomo vardą
        {//reiks daryti validaciją kad priimtų tik su unikaliais vardais
            room.roomAdminId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            var roomie = _mapper.Map<Room>(room);
            try
            {
                var _room = _roomService.Create(roomie,room.users);//reiks įvesk validaciiją dėl vardų
                List<int> users = new List<int>();
                users.Add(room.roomAdminId);
                App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 5, roomId = room.roomId, registered_room_users = users });
                _logService.Create(room.roomAdminId, room.roomId, 102, room.roomName);
                return Ok(_room);//gražina kad sėkmingai registruota
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //// GET: Rooms/user_get_rooms
        //[HttpGet("user_get_rooms")]
        //public IActionResult UsersGetRooms(JObject list)
        //{
        //    List<int> rooms = (list.Value<JArray>("rooms")).ToObject<List<int>>();
        //    string idString = Request.HttpContext.User.Identity.Name;//ima requesterio id
        //    var _rooms = _roomService.GetRoomsUsers(rooms,idString);//ima tik tuos roomus kuriuose jis registruotas
        //    return Ok(_rooms);//gražina roomus su roomId roomAdminId roomName users
        //}

        // GET: Rooms/user_get_rooms
        [HttpGet("user_get_rooms")]
        public IActionResult UsersGetRooms()
        {
            
            string id = Request.HttpContext.User.Identity.Name;
            //var _rooms = _roomService.GetRoomsUsers(rooms, idString);//ima tik tuos roomus kuriuose jis registruotas
            var rooms = _roomService.GetAllRooms().Where(x => x.users != null && x.users.Contains(Int32.Parse(id))).Select(y => new { guid = y.guid, roomAdminId = y.roomAdminId, roomId = y.roomId, roomName = y.roomName});
            return Ok(rooms);
        }

        // GET: Rooms/admin_get_rooms
        [HttpGet("admin_get_rooms")]
        public IActionResult AdminGetRooms()
        {
            string idString = Request.HttpContext.User.Identity.Name;//ima requesterio id
            var _rooms = _roomService.GetRoomsAdmin(idString);//ima tik tuos rūmus kuriuose jis adminas
            return Ok(_rooms);//gražina roomus su roomId roomAdminId roomName guid users
        }
        // PUT: Rooms/kick_user
        [HttpPut("kick_user")]
        public IActionResult KickUser(JObject data)
        {
            try
            {
                int roomId = data.Value<int>("roomId");
                int userToKick = data.Value<int>("userId");
                int admin = Int32.Parse(Request.HttpContext.User.Identity.Name);
                _roomService.KickUser(roomId,userToKick,admin);
                //RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == roomId);
                List<int> usersToKick = new List<int>();
                usersToKick.Add(userToKick);
                //registeredUsers.Add(room.roomAdminId);
                App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 4, roomId = roomId, registered_room_users = usersToKick});
                _logService.Create(admin, userToKick, 103, _roomService.GetRoom(roomId).roomName);//What user was kicked???
                _logService.Create(userToKick, admin, 106, _roomService.GetRoom(roomId).roomName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
                // PUT: Rooms/leave
        [HttpPut("leave")]
        public IActionResult LeaveRoom(JObject data)
        {
            try
            {
                int roomId = data.Value<int>("roomId");
                int user = Int32.Parse(Request.HttpContext.User.Identity.Name);
                _roomService.LeaveRoom(user,roomId);
                //RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == roomId);
                List<int> usersToKick = new List<int>();
                usersToKick.Add(user);
                //registeredUsers.Add(room.roomAdminId);
                App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 4, roomId = roomId, registered_room_users = usersToKick});                
                RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == roomId);
                List<int> registeredUsers = new List<int>();
                if (room.users != null)
                    registeredUsers.AddRange(room.users);
                registeredUsers.Add(room.roomAdminId);
                App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 3, roomId = roomId, registered_room_users = registeredUsers });
                _logService.Create(user, roomId, 105, room.roomName);
                _logService.Create(room.roomAdminId, user, 107, room.roomName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        // PUT: Rooms/join_group
        [HttpPut("join_group")]
        public IActionResult JoinGroup(JObject guid)
        {
            try//
            {
            string _guid = guid.Value<string>("guid");
            int id = Convert.ToInt32(Request.HttpContext.User.Identity.Name);//gauna authentifikuoto asmens id, nes taip saugiau
            
            Room temp = _roomService.JoinGroup(id,_guid);//įrašo į roomo userių šąrašą ir userio roomų sąrašą
                if (temp == null)
                {
                    throw new AppException( "Room not found");
                }

                RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == temp.roomId);
                List<int> registeredUsers = new List<int>();
                if (room.users != null)
                    registeredUsers.AddRange(room.users);
                registeredUsers.Add(room.roomAdminId);
                App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 3, roomId = temp.roomId, registered_room_users = registeredUsers });
                _logService.Create(id, room.roomId, 104, room.roomName);
                _logService.Create(room.roomAdminId, id, 108, room.roomName);
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
            try
            {
                int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
                RoomDto room = _roomService.GetRoom(id);
                if (room != null)
                {
                    _roomService.Delete(id, requesterId);//tik roomo adminas gali ištrinti roomą
                    _logService.Create(requesterId, id, 101, room.roomName);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        [AllowAnonymous]//Pratestuoti
        [Route("login_group/{RoomId:int}")]
        public IActionResult LoginToGroup(int RoomId)
        {
            //ar RoomId roomas egzistuoja?\
            if(!(_roomService.GetAllRooms().Any(x => x.roomId == RoomId)))
            {
                //throw new AppException("Room with that ID does not exist!");
                return Content("Room with that ID does not exist!");
            }
            //patikrinti ar useris priklauso roomui
            //
            //
            //
            int UserId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            TempRoom tempRoom;//sukuriamas specialus objektas roomu stebejimui ir valdymui sesijos metu.
            if (App.Inst.tempRooms.Any(x => x.usersById.Any(y => y.Key == UserId)))//jei useris priklauso jau kazkuriam roomui, reikia jam neleisti prisijungti arba atjungti reiketu pries prijungiant. Kolkas nieko nedarysiu.
            {
                //throw new AppException("User is already logged in!");
                return Content("User is already logged in!");
            }
            if (!(tempRoom = App.Inst.tempRooms.Find(x => x.roomId == RoomId))?.usersById.Any(y => y.Key == UserId) ?? true)//tikrina ta rooma, i kuri bando useris jungtis. Tikrinama ar useris ten jau nera prisijunges
            {
                if (tempRoom == null)
                {
                    tempRoom = new TempRoom(RoomId);
                    App.Inst.tempRooms.Add(tempRoom);
                }
                tempRoom.usersById.Add(UserId, "A");//reikes padaryti kad uzkrautu paskutini userio statusa.
                //
                //App.Inst.Add(UserId, RoomId);
                //
                
                //raise room modified event?
            }
            else
            {
                //throw new AppException("User is already logged into that room!");
                return Content("User is already logged into that room!");
            }

            RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == RoomId);
            List<int> registeredUsers = new List<int>();
            if (room.users != null)
            {
                registeredUsers.AddRange(room.users);
            }
            registeredUsers.Add(room.roomAdminId);
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 2, roomId = RoomId, registered_room_users = registeredUsers, senderId = UserId, data = "1" });// data = 1, user logged in.
            return Ok();
        }

        [AllowAnonymous]//Pratestuoti
        [Route("logout_group/{RoomId:int}")]
        public IActionResult LogoutFromGroup(int RoomId)
        {
            int UserId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            TempRoom tempRoom;
            if ((tempRoom = App.Inst.tempRooms.Where(x => x.usersById.Keys.Contains(UserId)).FirstOrDefault()) == null)//jei useris priklauso jau kazkuriam roomui, reikia jam neleisti prisijungti arba atjungti reiketu pries prijungiant. Kolkas nieko nedarysiu.
            {
                //throw new AppException("User is already logged out!");
                return Content("User is already logged out!");
                //return ResponseMessage();
                //Content()
            }
            else
            {
                //App.Inst.tempRooms.Where(x=>x.roomId==RoomId&&x.usersById.ContainsKey(UserId));
                tempRoom.usersById.Remove(UserId);
                //App.Inst.Remove(UserId);
                if (tempRoom.usersById.Count == 0)
                {
                    App.Inst.tempRooms.Remove(tempRoom);
                }
            }

            RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == RoomId);
            List<int> registeredUsers = new List<int>();
            if (room.users != null)
                registeredUsers.AddRange(room.users);
            registeredUsers.Add(room.roomAdminId);
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 2, roomId = RoomId, registered_room_users = registeredUsers, senderId = UserId, data = "0" });//data = 0, user logged out.
            return Ok();
        }

        [Route("group_members/{RoomId:int}/{Format:bool}")]
        public IActionResult GroupMembersInfo(int RoomId, bool Format)
        {
            RoomDto room = _roomService.GetRoom(RoomId);
            if (room != null)
            {
                List<int> users = room.users;

                if (Format)
                {
                    if (users == null)
                    {
                        users = new List<int>();
                    }
                    users.Add(room.roomAdminId);
                }

                var userList = users?.Select(x =>
                {
                    User user = _userService.GetById(x);
                    return new { id = user.Id, username = user.Username, firstName = user.FirstName, lastName = user.LastName, token = "" };
                });

                return Ok(userList);
            }
            return BadRequest();
        }

        [AllowAnonymous]//Pratestuoti
        [Route("group/{RoomId:int}")]
        public IActionResult GroupInfo(int RoomId)
        {
            //reikia is db pasiimti visus userius
            //for (int i = 1; i < 5; i++)
            //{
            //    TempRoom t;
            //    App.Inst.tempRooms.Add(t = new TempRoom(i));
            //    t.usersById.Add(i + 3 * (i - 1), "A");
            //    t.usersById.Add((i + 1) + 3 * (i - 1), "A");
            //    t.usersById.Add((i + 2) + 3 * (i - 1), "A");
            //}
            //Patikrinti ar useris vis dar roome prisijunges.
            var o = App.Inst.tempRooms.FirstOrDefault(x => x.roomId == RoomId)?.usersById.Select(y => new {key = _userService.GetAll().FirstOrDefault(z => z.Id == y.Key), value = y.Value });
            return Ok(o);//prideti userio statusa
        }

        [Route("status/{roomId:int}/{status:alpha}")]
        public IActionResult UserStatusChange(int roomId, string status)
        {
            //regex match
            int UserId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            TempRoom room = App.Inst.tempRooms.Find(x => x.roomId == roomId);
            if (room != null)
            {
                if (room.usersById.Remove(UserId))
                {
                    room.usersById.Add(UserId, status);
                }
            }

            RoomDto roomDto = _roomService.GetAllRooms().First(x => x.roomId == roomId);
            List<int> registeredUsers = new List<int>();
            if (roomDto.users != null)
                registeredUsers.AddRange(roomDto.users);
            registeredUsers.Add(roomDto.roomAdminId);
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 2, roomId = roomId, registered_room_users = registeredUsers, senderId = UserId, data = new Func<string, string>((s) => { switch (s) { case "A": { return "2"; } case "B": { return "3"; } case "C": { return "4"; } default: { return "4"; } } })(status)  });
            return Ok();
        }

        [Route("change_guid/{roomId:int}")]
        public IActionResult ChangeRoomGuid(int roomId)
        {
            int userId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            Guid? new_guid = _roomService.UpdateGuid(roomId, userId);
            if (new_guid != null)
            {
                _logService.Create(userId, roomId, 100);
                return Ok(new_guid);
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("roomname/{roomId:int}")]
        public IActionResult GetUsername(int roomId)
        {
            return Ok(_roomService.GetRoom(roomId).roomName);
        }

        [Route("online_members/{roomId:int}")]
        public IActionResult GetOnlineUsersOfRoom(int roomId)
        {
            return Ok(App.Inst.tempRooms.FirstOrDefault(x => x.roomId == roomId)?.usersById.Select(y => y.Key));
        }
    }
}
