using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IChatLineService
    {
        List<ChatLine> GetRoomChat(int roomId);
        void Create(int roomId, int creatorId, string text);
    }
    public class ChatLineService : IChatLineService
    {

        private DataContext _context;

        public ChatLineService(DataContext context)
        {
            _context = context;
        }

        public void Create(int roomId, int creatorId, string text)
        {
            ChatLine line = new ChatLine() { RoomId = roomId, CreateDate = DateTime.Now, CreatorId = creatorId, Text = text };
            _context.ChatLines.Add(line);
            if (_context.ChatLines.Count() > 100)
            {
                _context.ChatLines.Remove(_context.ChatLines.OrderBy(x => x.CreateDate).First());
            }
            _context.SaveChanges();
        }

        public List<ChatLine> GetRoomChat(int roomId)
        {
            return _context.ChatLines.Where(x => x.RoomId == roomId).ToList();
        }
    }
}
