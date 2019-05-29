using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : Controller
    {
        private IMapper _mapper;
        private IRoomService _roomService;
        private IUserService _userService;
        private INoteService _noteService;

        public NotesController(IRoomService roomService, IMapper mapper, IUserService userService, INoteService noteService)
        {
            _roomService = roomService;
            _mapper = mapper;
            _userService = userService;
            _noteService = noteService;
        }

        [Route("{roomId:int}")]
        public IActionResult GetNotes(int roomId)
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            List<Note> notes = new List<Note>();
            RoomDto room;
            if ((room = _roomService.GetById(roomId, requesterId)) != null)
            {
                notes = _noteService.GetNotesForRoom(room.roomId);
            }
            return Ok(notes);
        }

        [Route("submit")]
        [HttpPost]
        public IActionResult SubmitNote(JObject info)
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            Dictionary<string, string> info_dic = info.ToObject<Dictionary<string, string>>();
            try
            {
                _noteService.Create(info_dic["message"], requesterId, Int32.Parse(info_dic["roomId"]), info_dic["header"]);
            }
            catch(Exception exception)
            {
                return Content("Counld not create note!");
            }
            int roomId = Int32.Parse(info_dic["roomId"]);
            RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == roomId);
            List<int> registeredUsers = new List<int>();
            if (room.users != null)
            {
                registeredUsers.AddRange(room.users);
            }  
            registeredUsers.Add(room.roomAdminId);
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 0, roomId = roomId, registered_room_users = registeredUsers});
            return Ok();
        }

        [Route("delete/{roomId:int}/{noteId:int}")]
        public IActionResult Delete(int roomId, int noteId)
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);//Validation?
            try
            {
                _noteService.Delete(roomId, noteId);
            }
            catch (Exception exception)
            {
                return Content("Counld not create note!");
            }

            RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == roomId);
            List<int> registeredUsers = new List<int>();
            if (room.users != null)
                registeredUsers.AddRange(room.users);
            registeredUsers.Add(room.roomAdminId);
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 0, roomId = roomId, registered_room_users = registeredUsers });
            return Ok();
        }

        [Route("modify")]
        [HttpPost]
        public IActionResult ModifyNote([FromBody]Note note)
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            try
            {
                _noteService.Modify(note);
            }
            catch (Exception exception)
            {
                return Content("Counld not create note!");
            }

            RoomDto room = _roomService.GetAllRooms().First(x => x.roomId == note.RoomId);
            List<int> registeredUsers = new List<int>();
            if (room.users != null)
                registeredUsers.AddRange(room.users);
            registeredUsers.Add(room.roomAdminId);
            App.Inst.RaiseRoomchangedEvent(this, new ChangeEventArgs() { change = 0, roomId = note.RoomId, registered_room_users = registeredUsers });
            return Ok();
        }
    }
}