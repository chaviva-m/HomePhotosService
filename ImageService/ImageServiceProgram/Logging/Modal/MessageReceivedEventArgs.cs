using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Logging.Modal
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }
        public MessageReceivedEventArgs(MessageTypeEnum status, string msg)
        {
            Status = status;
            Message = msg;
        }
    }
}
