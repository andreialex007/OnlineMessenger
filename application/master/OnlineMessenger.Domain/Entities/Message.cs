using System;
using System.Collections.Generic;

namespace OnlineMessenger.Domain.Entities
{
    public class Message
    {
        public Message()
        {
            Date = DateTime.Now;
            To = new List<User>();
        }
         
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public int FromId { get; set; }
        public virtual User From { get; set; }
        public virtual List<User> To { get; set; }
    }
}