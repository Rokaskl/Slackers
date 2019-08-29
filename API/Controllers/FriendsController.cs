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
        private readonly INotificationService _notificationService;
        private readonly ILogService _logService;

        public FriendsController(DataContext context, IFriendshipService friendshipService, IFriendshipRequestService friendshipRequestService, IUserService userService, INotificationService notificationService, ILogService logService)
        {
            _context = context;
            _friendshipService = friendshipService;
            _friendshipRequestService = friendshipRequestService;
            _userService = userService;
            _notificationService = notificationService;
            _logService = logService;
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
            return Ok(_userService.UserViewModel_dict(_friendshipService.Search(userId, search_string), false));
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
            App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = sender, change = 6, receivers = new List<int>() { receiver}, data = "0"});
            _notificationService.LeaveNotification(receiver, 0);
            _logService.Create(receiver, sender, 0);
            _logService.Create(sender, receiver, 7);
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
            if(_friendshipRequestService.Delete(sender, receiver))
            {
                App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = sender, change = 6, receivers = new List<int>() { receiver }, data = "1" });
                _notificationService.LeaveNotification(receiver, 1);
                _logService.Create(receiver, sender, 1);
                _logService.Create(sender, receiver, 8);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
        }

        /// <summary>
        /// Sender sends request to cancel his friendship request to receiver.
        /// </summary>
        /// <param name="sender">sender id</param>
        /// <param name="receiver">receiver id</param>
        /// <returns>bool. true if request was successfully sent to server.</returns>
        [Route("request/{sender:int}/{receiver:int}/cancel")]
        [HttpGet]
        public IActionResult Cancel(int sender, int receiver)
        {
            if (_friendshipRequestService.Delete(sender, receiver))
            {
                App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = sender, change = 6, receivers = new List<int>() { receiver }, data = "6" });
                _notificationService.LeaveNotification(receiver, 6);
                _logService.Create(receiver, sender, 6);
                _logService.Create(sender, receiver, 9);
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
            if(_friendshipService.Delete(sender, receiver))
            {
                App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = sender, change = 6, receivers = new List<int>() { receiver }, data = "2" });
                _notificationService.LeaveNotification(receiver, 2);
                _logService.Create(receiver, sender, 2);
                _logService.Create(sender, receiver, 10);
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
            App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = sender, change = 6, receivers = new List<int>() { receiver }, data = "3" });
            _notificationService.LeaveNotification(receiver, 3);
            _logService.Create(receiver, sender, 3);
            _logService.Create(sender, receiver, 11);
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
            return Ok(_userService.UserViewModel_dict(_friendshipRequestService.RequestsOf_Outgoing(id), false));
        }

        /// <summary>
        /// Used to change user status which can be seen by his friends.
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="status">upcoming status</param>
        /// <returns>Ok if server received request and completed it without errors.</returns>
        [Route("status/{sender:int}/{status:int}")]
        [HttpGet]
        public IActionResult StatusChange(int sender, int status)//status = 1 - online; status = 0 - offline/invisible.
        {
            if (status == 1 && !App.Inst.OnlineStatusUsers.Contains(sender))
            {
                App.Inst.OnlineStatusUsers.Add(sender);
            }
            else
            {
                if (status == 0 && App.Inst.OnlineStatusUsers.Contains(sender))
                {
                    App.Inst.OnlineStatusUsers.Remove(sender);
                }
            }
            App.Inst.RaiseFriendschangedEvent(this, new FriendsChangeEventArgs() { senderId = sender, change = 6, receivers = new List<int>(_friendshipService.Friends_and_RequestReceivers_Of(sender)), data = status == 1 ? "4" : "5" });
            return Ok();
        }


        [Route("user/{userId:int}")]
        [HttpGet]
        public IActionResult UserInfo(int userId)
        {
            return Ok(_userService.UserViewModel_dict(new List<int>() { userId }));
        }

        [Route("statuses")]
        public IActionResult GetUserStatuses([FromBody] List<int> ids)
        {
            return Ok(ids.Select(x =>
            {
                if (App.Inst.OnlineStatusUsers.Contains(x))
                {
                    return new KeyValuePair<int, bool>(x, true);
                }
                else
                {
                    return new KeyValuePair<int, bool>(x, false);
                }
            }
            ));
        }
    }
}