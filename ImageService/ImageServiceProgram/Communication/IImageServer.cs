using Communication.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Communication
{
    interface IImageServer
    {
        string SendClientCommand(TcpClient client, CommandReceivedEventArgs Args, out bool result);

    }
}
