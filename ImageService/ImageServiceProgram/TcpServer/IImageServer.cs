﻿using CommandInfrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.TcpServer
{
    interface IImageServer
    {
        string SendClientCommand(int id, CommandReceivedEventArgs Args, out bool result);
    }
}
