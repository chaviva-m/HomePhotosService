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

        /// <summary>
        /// constructor for messageReceivedEventArgs 
        /// </summary>
        /// <param name="status the type of the message"></param>
        /// <param name="msg the msg to write in the logger"></param>
        public MessageReceivedEventArgs(MessageTypeEnum status, string msg)
        {
            Status = status;
            Message = msg;
        }
    }
}
