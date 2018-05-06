
using ImageServiceProgram.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageReceivedEventArgs> MessageRecieved;    //server subscribes with "sendToClient" method to send new logs to client

        public LoggingService() {}

        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecieved?.Invoke(this, new MessageReceivedEventArgs(type, message));
        }
    }
}
