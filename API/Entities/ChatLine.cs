using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class ChatLine
    {
        public int Id { get; set; }
        public int Context_id { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Text { get; set; }
        public bool Room { get; set; }
    }
}
