﻿using CommandInfrastructure.Commands;
using ImageServiceProgram.Controller;
using ImageServiceProgram.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageServiceProgram.Handlers
{
    public interface IClientHandler
    {
        IImageController Controller { set; }
        event EventHandler<CommandReceivedEventArgs> CommandReceivedForHandlers;
        void HandleClient(TcpClient client, int clientID, ILoggingService logger);//, Mutex mutex);
    }
}
