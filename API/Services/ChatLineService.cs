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
        List<ChatLine> GetRoomChat(int roomId, int page, int items_per_page);
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

        public List<ChatLine> GetRoomChat(int roomId, int page, int items_per_page)
        {
            List<ChatLine> chatlines = _context.ChatLines.Where(x => x.RoomId == roomId).OrderByDescending(y => y.CreateDate).ToList();
            if (chatlines.Count < ((page + 1) * items_per_page))
            {
                //Nera tiek irasu...
                if (chatlines.Count < page * items_per_page)
                {
                    return new List<ChatLine>();
                }
                else
                {
                    return chatlines.GetRange(page * items_per_page, chatlines.Count - page * items_per_page);
                }
                
            }
            else
            {
                //Irasu pakanka.
                return chatlines.GetRange(page * items_per_page, items_per_page);
            }
            
        }
    }
}
