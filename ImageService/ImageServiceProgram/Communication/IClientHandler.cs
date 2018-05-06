using Communication.Commands;
using ImageServiceProgram.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Communication
{
    public interface IClientHandler
    {
        event EventHandler<CommandReceivedEventArgs> CommandReceivedForHandlers;
        void HandleClient(TcpClient client, ILoggingService logger);
    }
}
