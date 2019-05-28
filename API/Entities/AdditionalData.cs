using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Entities
{   
    public class AdditionalData
    {        
        
        public int Id{get;set;}

        public int Ownerid{get;set;}        
        public bool IsUser{get;set;}
        public string Biography{get;set;}
        public byte[] PhotoBytes{get;set;}
    }
}
