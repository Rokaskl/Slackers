using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public int User_Id { get; set; }//Belongs to
        public int Causer_Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Message_num { get; set; }
        public string Nickname { get; set; }
    }
}
