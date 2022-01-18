using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; } 
        public int SenderId { get; set; }   
        public string SenderUserName { get; set; }  
        public string SenderPhotoUrl { get; set; }
        public int RecepientId { get; set; }
        public string RecepientUserName { get; set; }   
        public string RecepientPhotoUrl { get; set; }  
        public string Content { get; set; } 
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}