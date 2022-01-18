using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; } 
        public int SenderId { get; set; }   
        public string SenderUserName { get; set; }  
        public AppUser Sender { get; set; } 
        public int RecepientId { get; set; }
        public string RecepientUserName { get; set; }   
        public AppUser Recepient { get; set; }  
        public string Content { get; set; } 
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }  = DateTime.Now;
        public bool SenderDeleted { get; set; }
        public bool RecepientDeleted { get; set; }
    }
}