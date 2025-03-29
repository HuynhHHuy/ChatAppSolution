using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Common.DTOs
{
    public class MessageDto
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
