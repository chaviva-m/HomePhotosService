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

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="server">IImageServer</param>
		public GetConfigCommand(IImageServer server)
        {
            this.server = server;
        }

		/// <summary>
		/// execute GetConfigCommand
		/// send client app config data
		/// </summary>
		/// <param name="args">args[1]=client id</param>
		/// <param name="result">result of command</param>
		/// <returns>return string indicating if command was successful</returns>
		public string Execute(string[] args, out bool result)
        {
			//get config data from AppConfigData
			AppConfigData confData = AppConfigData.Instance;
			//add data as strings to list
			//order of list: OutputDir, EventSourceName, LogName, ThumbnailSize, directory1, directory2 etc.
			List<string> data = new List<string>();
            data.Add(confData.OutputDir);
            data.Add(confData.EventSourceName);
            data.Add(confData.LogName);
            data.Add(confData.ThumbnailSize.ToString());
            string[] dat = data.ToArray();
            var conf = new string[dat.Length + confData.Directories.Length];
            dat.CopyTo(conf, 0);
            confData.Directories.CopyTo(conf, dat.Length);
			//rest of args for commandReceivedEventArgs
			int id = (int)CommandEnum.GetConfigCommand;
			string RequestDirPath = "";
			CommandReceivedEventArgs arg = new CommandReceivedEventArgs(id, conf, RequestDirPath);
			//send client the log history command and return result
			return server.SendClientCommand(int.Parse(args[1]), arg,out result);
        }
    }
}