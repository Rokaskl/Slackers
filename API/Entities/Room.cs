using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;
=======
>>>>>>> 050215db5888b4bd489eacf3e31ad7d19694412d
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace WebApi.Entities
{
    public class Room 
    {
<<<<<<< HEAD
        public int roomId { get; set; }
        public int roomAdminId { get; set; }
        public string roomName { get; set; }
        [NotMapped]
        public int[] users { get; set; }
=======
      

        public int roomId { get; set; }
        public int roomAdminId { get; set; }
        public string roomName { get; set; }
        
>>>>>>> 050215db5888b4bd489eacf3e31ad7d19694412d
    }
}
       
       


