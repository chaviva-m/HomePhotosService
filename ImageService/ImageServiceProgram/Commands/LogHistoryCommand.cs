﻿using Communication.Commands;
using Communication.Commands.Enums;
using ImageServiceProgram.Logging.Modal;
using ImageServiceProgram.Service;
using ImageServiceProgram.TcpServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Commands
{
    class LogHistoryCommand : ICommand
    {

        private IImageServer server;


        public LogHistoryCommand(IImageServer server)
        {
            this.server = server;
        }

        public string Execute(string[] args, out bool result)
        {
            LogTracker tracker = LogTracker.Instance;

            MessageReceivedEventArgs[] logList = tracker.LogList.ToArray();
            string requestDirPath = "";
            List<string> data = new List<string>();
            MessageReceivedEventArgs msg;
            string type;
            string message;
            for (int i = 0; i < logList.Length; i++)
            {
                msg = logList[i];
                type = msg.Status.ToString();
                message = msg.Message;
                data.Add(type);
                data.Add(message);

            }
            int id = (int)CommandEnum.LogHistoryCommand;
            CommandReceivedEventArgs arg = new CommandReceivedEventArgs(id, data.ToArray(), requestDirPath);
            return server.SendClientCommand(int.Parse(args[1]), arg, out result);

        }


    }
}