using CommandInfrastructure.Commands;
using CommandInfrastructure.Commands.Enums;
using ImageServiceProgram.TcpServer;
using ImageServiceProgram.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ImageServiceProgram.Commands
{
    class GetConfigCommand : ICommand
    {
        private IImageServer server;


        public GetConfigCommand(IImageServer server)
        {
            this.server = server;
        }

        public string Execute(string[] args, out bool result)
        {
            AppConfigData confData = AppConfigData.Instance;
            int id = (int)CommandEnum.GetConfigCommand;
            string RequestDirPath = "";
            List<string> data = new List<string>();
            data.Add(confData.OutputDir);
            data.Add(confData.EventSourceName);
            data.Add(confData.LogName);
            data.Add(confData.ThumbnailSize.ToString());
            string[] dat = data.ToArray();
            var conf = new string[dat.Length + confData.Directories.Length];
            dat.CopyTo(conf, 0);
            confData.Directories.CopyTo(conf, dat.Length);
            CommandReceivedEventArgs arg = new CommandReceivedEventArgs(id, conf, RequestDirPath);
            return server.SendClientCommand(int.Parse(args[1]), arg,out result);

        }
    }
}
