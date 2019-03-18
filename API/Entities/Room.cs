using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace WebApi.Entities
{
    public class Room 
    {
        public int roomId { get; set; }
        public int roomAdminId { get; set; }
        public string roomName { get; set; }
        [NotMapped]
        public int[] users { get; set; }
    }
}
       
       


