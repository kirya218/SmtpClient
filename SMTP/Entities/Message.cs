using System;
using System.Collections.Generic;

namespace SMTP.Entities
{
    public class Message
    {
        public string From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public string Body { get; set; }
        public string Subject { get; set; }
        public string FullMessage => DateTime.Now + "\r\n" + From + "\r\n" + To + "\r\n" + Subject + "\r\n" + Body;
    }
}
