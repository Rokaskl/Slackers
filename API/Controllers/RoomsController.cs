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
        public IActionResult GetRooms()
        {
            var _rooms = _roomService.GetAllRooms();;
            return Ok(_rooms);
        }

        // GET: Rooms/5
        [HttpGet("{id}")]
        public IActionResult GetRoom(int id)
        {
            var room = _roomService.GetById(id);     
            return Ok(room);
        }
        // POST: Rooms
        [HttpPost("register")]
        public IActionResult Register([FromBody]RoomDto room)
        {
            var roomie = _mapper.Map<Room>(room);
            try
            {
            _roomService.Create(roomie,room.users);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: Rooms/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            _roomService.Delete(id);                
            return Ok();
        }
    }
}
