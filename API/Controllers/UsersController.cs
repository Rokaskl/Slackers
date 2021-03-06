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
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private IFriendshipService _friendshipService;

        public UsersController(IUserService userService,IMapper mapper,IOptions<AppSettings> appSettings, IFriendshipService friendshipService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _friendshipService = friendshipService;
            //TestSeed(20); // Kiek randomu sugeneruoti testavimui
        }

        public void TestSeed(int n)
        {           
            string y;
            for (int i = 0; i < n; i++)
            {
                y = i.ToString();
                Register(new UserDto("Vardas" + y, "Pavarde" + y, "User" + y, "Password" + y));
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate/{web}")]
        public IActionResult Authenticate(int web, [FromBody]UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Username, userDto.Password, web == 1);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //prideda sekmingai prisijungusi vartotoja prie serverio loggedin useriu saraso.
            //App.Inst.loggedin.Add(user.Id);
            App.Inst.Add(user.Id);

            if (!App.Inst.OnlineStatusUsers.Contains(user.Id))
            {
                App.Inst.OnlineStatusUsers.Add(user.Id);
            }

            App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = user.Id, change = 6, receivers = new List<int>(_friendshipService.Friends_and_RequestReceivers_Of(user.Id)), data = "4" });

            // return basic user info (without password) and token to store client side
            return Ok(new {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Rooms = user.rooms,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try 
            {
                // save 
                var temp =  _mapper.Map<UserDto>(_userService.Create(user, userDto.Password));
                return Ok(temp);
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll()//reiks ištrint arba paliktri kad gražintų tik vardus
        {
            var users =  _userService.GetAll();
            return Ok(users);
        }        
        [HttpGet("{id}")]
        public IActionResult GetById(int id)//same kap viršesnio ^^^
        {
            var user =  _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }                     
        [HttpPost("get_list")]
        public IActionResult GetList(JObject users)//same kap viršesnio ^^^
        {
            List<int> ids = users.Value<JArray>("ids").ToObject<List<int>>();
            
            //List<int> IDS= ids.AsParallel().ToList<int>();
            List<User> user =  _userService.GetList(ids );
            return Ok(user);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDto userDto)//gal pakeisti reikės 
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.Id = id;

            try 
            {
                // save 
                _userService.Update(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [Route("logout")]
        public IActionResult Logout()
        {
            int requesterId = Convert.ToInt32(Request.HttpContext.User.Identity.Name);
            App.Inst.Remove(requesterId);
            App.Inst.OnlineStatusUsers.Remove(requesterId);
            App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = requesterId, change = 6, receivers = new List<int>(_friendshipService.Friends_and_RequestReceivers_Of(requesterId)), data = "5" });
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            int id = Convert.ToInt32(Request.HttpContext.User.Identity.Name);//pašalina prisijungusį vartotoją
            _userService.Delete(id);
            return Ok();
        }
    }
}
