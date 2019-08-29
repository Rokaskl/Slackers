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
        void Create(int roomId, int creatorId, string text, bool room = true);
        List<ChatLine> GetUserChat(int userId, int senderId, int page, int items_per_page);
    }
    public class ChatLineService : IChatLineService
    {

        private DataContext _context;

        public ChatLineService(DataContext context)
        {
            _context = context;
        }

        public void Create(int room_or_user_id, int creatorId, string text, bool room = true)
        {
            ChatLine line = new ChatLine() { Context_id = room_or_user_id, CreateDate = DateTime.Now, CreatorId = creatorId, Text = text, Room = room};
            _context.ChatLines.Add(line);
            //if (_context.ChatLines.Count() > 100)
            //{
            //    _context.ChatLines.Remove(_context.ChatLines.OrderBy(x => x.CreateDate).First());
            //}
            _context.SaveChanges();
        }

        public List<ChatLine> GetRoomChat(int roomId, int page, int items_per_page)
        {
            List<ChatLine> chatlines = _context.ChatLines.Where(x => x.Context_id == roomId && x.Room).OrderByDescending(y => y.CreateDate).ToList();
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

        public List<ChatLine> GetUserChat(int userId, int senderId, int page, int items_per_page)
        {
            var ccc = _context.ChatLines.Where(x => x.Id > 0).ToList();
            List<ChatLine> chatlines = _context.ChatLines.Where(x => !x.Room && (((x.Context_id == userId && x.CreatorId == senderId) || (x.Context_id == senderId && x.CreatorId == userId)))).OrderByDescending(y => y.CreateDate).ToList();
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
