using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFriendshipService _friendshipService;
        private readonly IFriendshipRequestService _friendshipRequestService;
        private readonly IUserService _userService;

        public FriendsController(DataContext context, IFriendshipService friendshipService, IFriendshipRequestService friendshipRequestService, IUserService userService)
        {
            _context = context;
            _friendshipService = friendshipService;
            _friendshipRequestService = friendshipRequestService;
            _userService = userService;
        }

        /// <summary>
        /// Generates FriendList of {id} person from Friendship entities.
        /// </summary>
        /// <param name="id">Requester id</param>
        /// <returns>A list of Users (photo, username, status, id)</returns>
        [Route("{userId:int}")]
        [HttpGet]
        public IActionResult FriendsList(int userId)
        {
            return Ok(_userService.UserViewModel_dict(_friendshipService.FriendsOf(userId)));
        }

        /// <summary>
        /// Friend search method.
        /// </summary>
        /// <param name="search_string">Used to match peoples' usernames with it</param>
        /// <returns>Found people using search string(username matched or is similar)</returns>
        [Route("search/{userId:int}/{search_string}")]
        [HttpGet]
        public IActionResult Search(int userId, string search_string)
        {
            return Ok(_userService.UserViewModel_dict(_friendshipService.Search(userId, search_string)));
        }

        /// <summary>
        /// Sender sends request to add receiver to his friends list.
        /// </summary>
        /// <param name="sender">sender id</param>
        /// <param name="receiver">receiver id</param>
        /// <returns>bool. true if request was successfully sent to server.</returns>
        [Route("request/{sender:int}/{receiver:int}/add")]
        [HttpGet]
        public IActionResult Add(int sender, int receiver)
        {
            _friendshipRequestService.Create(sender, receiver);
            //Notification
            return Ok();
        }

        /// <summary>
        /// Sender sends request to reject receiver's friendship request.
        /// </summary>
        /// <param name="sender">sender id</param>
        /// <param name="receiver">receiver id</param>
        /// <returns>bool. true if request was successfully sent to server.</returns>
        [Route("request/{sender:int}/{receiver:int}/reject")]
        [HttpGet]
        public IActionResult Reject(int sender, int receiver)
        {
            //Notification
            if(_friendshipRequestService.Delete(sender, receiver))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
        }

        /// <summary>
        /// Sender sends request to end friendship with {receiver} user.
        /// </summary>
        /// <param name="sender">sender id</param>
        /// <param name="receiver">receiver id</param>
        /// <returns>bool. true if request was successfully sent to server.</returns>
        [Route("request/{sender:int}/{receiver:int}/remove")]
        [HttpGet]
        public IActionResult Remove(int sender, int receiver)
        {
            //Notification
            if(_friendshipService.Delete(sender, receiver))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Sender sends request to accept receiver's friendship request.
        /// </summary>
        /// <param name="sender">sender id</param>
        /// <param name="receiver">receiver id</param>
        /// <returns>bool. true if request was successfully sent to server.</returns>
        [Route("request/{sender:int}/{receiver:int}/accept")]
        [HttpGet]
        public IActionResult Accept(int sender, int receiver)
        {
            _friendshipRequestService.Delete(sender, receiver);
            _friendshipService.Create(sender, receiver);
            //Notification
            return Ok();
        }

        /// <summary>
        /// Checks and generates a list of incoming requests to become friends for the user.
        /// </summary>
        /// <param name="id">requester id</param>
        /// <returns>list of requests to become friends.</returns>
        [Route("requests/{id:int}/in")]
        [HttpGet]
        public IActionResult Requests_Incoming(int id)
        {
            return Ok(_userService.UserViewModel_dict(_friendshipRequestService.RequestsOf_Incoming(id)));
        }

        /// <summary>
        /// Checks and generates a list of outgoing requests to become friends for the user.
        /// </summary>
        /// <param name="id">requester id</param>
        /// <returns>list of requests to become friends.</returns>
        [Route("requests/{id:int}/out")]
        [HttpGet]
        public IActionResult Requests_Outgoing(int id)
        {
            return Ok(_userService.UserViewModel_dict(_friendshipRequestService.RequestsOf_Outgoing(id)));
        }

        /// <summary>
        /// Used to change user status which can be seen by his friends.
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="status">upcoming status</param>
        /// <returns>Ok if server received request and completed it without errors.</returns>
        [Route("status/{id:int}/{status:int}")]
        [HttpGet]
        public IActionResult StatusChange(int id, int status)//status = 1 - online; status = 0 - offline/invisible.
        {
            //Notification
            if (status == 1 && !App.Inst.OnlineStatusUsers.Contains(id))
            {
                App.Inst.OnlineStatusUsers.Add(id);
            }
            else
            {
                if (status == 0 && App.Inst.OnlineStatusUsers.Contains(id))
                {
                    App.Inst.OnlineStatusUsers.Remove(id);
                }
            }
            return Ok();
        }
    }
}