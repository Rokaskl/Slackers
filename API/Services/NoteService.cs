using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface INoteService
    {
        //RoomDto GetById(int id, int requesterId);
        void Create(string message, int creatorId, int roomId, string header);
        //void Delete(int id, int requesterId);
        List<Note> GetAllNotes();
        List<Note> GetNotesForRoom(int roomId);
        bool Delete(int roomId, int noteId);
        bool Modify(Note note);
        //Room JoinGroup(int id, string uid);
        //List<RoomDto> GetRoomsUsers(List<int> ids, string idString);
        //List<RoomDto> GetRoomsAdmin(string idStrings);
    }
    public class NoteService : INoteService
    {
        private DataContext _context;

        public NoteService(DataContext context)
        {
            _context = context;
        }

        public void Create(string message, int creatorId, int roomId, string header)
        {
            //if (_context.Rooms.Any(x => x.roomName == room.roomName))//valiidacijja
            // throw new AppException("Room name \"" + room.roomName + "\" is already taken");

            //_context.TimeMarks

            //Guid g;
            //g = Guid.NewGuid();

            //byte[] _users = ConvertToBytes(users);

            //room.usersBytes = _users;
            //room.guid = g.ToString();

            _context.Notes.Add(new Note { Message = message, CreatorId = creatorId, RoomId = roomId, Header = header });
            _context.SaveChanges();

            //return room;
        }



        public List<Note> GetAllNotes()
        {
            //List<Note> notes = new List<Note>();
            //foreach (Note note in _context.Notes)
            //{
            //    //(int UserId, bool Action, int RoomId, DateTime Time)
            //    notes.Add(new Note());
            //    //TimeMarkDto markDto = new TimeMarkDto();
            //    //markDto.Action = mark.Action;
            //    //markDto.RoomId = mark.RoomId;
            //    //markDto.Time = mark.Time;
            //    //markDto.UserId = mark.UserId;
            //    //marks.Add(markDto);
            //}
            return _context.Notes.ToList();
        }

        public List<Note> GetNotesForRoom(int roomId)
        {
            return _context.Notes.Where(x => x.RoomId == roomId).ToList();
        }

        public bool Delete(int roomId, int noteId)
        {
            try
            {
                Note note = _context.Notes.Where(x => x.RoomId == roomId).Where(y => y.Id == noteId).First();
                _context.Notes.Remove(note);
                _context.SaveChanges();
                return true;
            }
            catch(Exception exception)
            {
                return false;
            }
        }

        public bool Modify(Note note)
        {
            try
            {
                Note old_note = _context.Notes.Where(x => x.RoomId == note.RoomId).Where(y => y.Id == note.Id).First();
                old_note.Message = note.Message;
                old_note.Header = note.Header;
                _context.SaveChanges();
                return true;
            }
            catch(Exception exception)
            {
                return false;
            }
        }
    }
}
