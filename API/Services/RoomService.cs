using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IRooService
    {
        Room get(RoomDto room);
    }
    public class RoomService:IRooService
    {
        private DataContext _context;

        public RoomService(DataContext context)
        {
            _context = context;
        }

        public Room get(RoomDto room)
        {
            return null;
        }

    }
}
