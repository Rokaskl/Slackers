using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Route("post/{userId:int}")]
        [HttpPost]
        public IActionResult Create(int userId, [FromBody] byte[] notifications_array)
        {
            if(_notificationService.Create(userId, notifications_array))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("{userId:int}")]
        [HttpGet]
        public IActionResult GetNotifications(int userId)
        {
            return Ok(_notificationService.Notifications_of(userId) ?? new byte[0]);
        }
    }
}