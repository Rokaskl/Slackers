using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface ILogService
    {
        bool Create(int user_id, int causer_id, int message_num, string nickname = null);
        List<Log> GetLogs(int user_id, int start_num, int count);
        Log GetSingleLogLine(int id);
    }

    public class LogService : ILogService
    {
        private DataContext _context;

        public LogService(DataContext context)
        {
            _context = context;
        }

        public bool Create(int user_id, int causer_id, int message_num, string nickname = null)
        {
            Entities.Log log = new Entities.Log();
            log.Causer_Id = causer_id;
            log.DateTime = DateTime.Now;
            log.Message_num = message_num;
            log.User_Id = user_id;
            log.Nickname = nickname;
            _context.Logs.Add(log);
            _context.SaveChanges();
            App.Inst.RaiseLogCreatedEvent(this, new FriendsChangeEventArgs() { senderId = log.Id, change = 6, receivers = new List<int>() { user_id }, data = "7" });
            return true;
        }

        public List<Log> GetLogs(int user_id, int start_num, int count)
        {
            return _context.Logs.Where(x => x.User_Id == user_id).OrderByDescending(y => y.DateTime).Skip(start_num).Take(count).ToList();
        }

        public Log GetSingleLogLine(int id)
        {
            return _context.Logs.Find(id);
        }
    }
}
