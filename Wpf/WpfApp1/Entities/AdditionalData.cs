using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{   
    public class AdditionalData
    {        
        
        public int Id{get;set;}

        public int Ownerid{get;set;}        
        public bool IsUser{get;set;}
        public string Biography{get;set;}
        public byte[] PhotoBytes{get;set;}

        public AdditionalData(int ownerid, bool isUser, string biography, byte[] photoBytes)
        {
            Ownerid = ownerid;
            IsUser = isUser;
            Biography = biography;
            PhotoBytes = photoBytes;
        }
    }
}
