using ImageServiceProgram.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Logging
{
    public interface ILoggingService
    {
        /// <summary>
        /// event invoked on message receival
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageRecieved;
        /// <summary>
        /// to log a message
        /// </summary>
        /// <param name="message">massage information</param>
        /// <param name="type">message type</param>
        void Log(string message, MessageTypeEnum type);           // Logging the Message
    }
}
