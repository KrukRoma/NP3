using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientChat
{
    public class MessageInfo
    {
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public MessageInfo(string? text, DateTime time)
        {
            Text = text;    
            Time = time;    
        }
        public override string ToString()
        {
            return $"{Text}  Date : {Time}";
        }
    }
}
