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
    public class LogsController : Controller
    {
        private ILogService _logService;
        private IUserService _userService;

        public LogsController(ILogService logService, IUserService userService)
        {
            _logService = logService;
            _userService = userService;
        }

        [Route("{userId:int}/{start_num:int}/{count:int}")]
        [HttpGet]
        public IActionResult GetLogs(int userId, int start_num, int count)
        {
            return Ok(_logService.GetLogs(userId, start_num, count));
        }

        [Route("username/{userId:int}")]
        public IActionResult GetUsername(int userId)
        {
            return Ok(_userService.GetById(userId).Username);
        }

        [Route("logline/{Id:int}")]
        public IActionResult GetSingleLogLine(int Id)
        {
            return Ok(_logService.GetSingleLogLine(Id));
        }
    }
}