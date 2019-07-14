using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ChatLineController : Controller
    {
        private IChatLineService _chatLineService;
        private IUserService _userService;
        private IRoomService _roomService;

        public ChatLineController(IChatLineService chatLineService, IUserService userService, IRoomService roomService)
        {
            _chatLineService = chatLineService;
            _userService = userService;
            _roomService = roomService;
        }

        [Route("create/{roomId:int}")]
        [HttpPost]
        public IActionResult Create(int roomId, [FromBody]string text)
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            //string text = info.ToObject<string>();
            //string text = info.ToObject<string>();
            try
            {
                _chatLineService.Create(roomId, requesterId, text);
            }
            catch(Exception exception)
            {
                return BadRequest(exception.Message);
            }

            RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == roomId);
            List<int> registeredUsers = new List<int>();
            if (room.users != null)
            {
                registeredUsers.AddRange(room.users);
            }
            registeredUsers.Add(room.roomAdminId);
            registeredUsers.Remove(requesterId);//Isimamas eilutes kurejas, jam atnaujinama lokaliai.
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 1, roomId = roomId, registered_room_users = registeredUsers, data = text, senderId = requesterId });
            return Ok();
        }

        [Route("lines/{roomId:int}/{page:int}/{items_per_page:int}")]
        public IActionResult GetChat(int roomId, int page, int items_per_page)
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);

            //return Ok(_chatLineService.GetRoomChat(roomId).Select(x => new { key = x, value = _userService.GetById(x.CreatorId).Username}));
            List<Dictionary<string, object>> lines = _chatLineService.GetRoomChat(roomId, page, items_per_page).Select(x => x.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(x, null))).ToList();
            foreach (Dictionary<string, object> line in lines){
                line.Add("Username", _userService.GetById(int.Parse(line["CreatorId"].ToString())).Username);
            }
          return Ok(lines);
        }
    }
}