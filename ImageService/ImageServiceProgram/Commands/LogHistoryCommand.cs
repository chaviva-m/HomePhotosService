using CommandInfrastructure.Commands;
using CommandInfrastructure.Commands.Enums;
using ImageServiceProgram.Logging.Modal;
using ImageServiceProgram.Logging;
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

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="server"></param>
        public LogHistoryCommand(IImageServer server)
        {
            this.server = server;
        }

		/// <summary>
		/// execute log history command
		/// send client the log history
		/// </summary>
		/// <param name="args">args[1]=client id</param>
		/// <param name="result">result of command</param>
		/// <returns>return string indicating if command was successful</returns>
		public string Execute(string[] args, out bool result)
        {
			//get log history from log tracker
			LogTracker tracker = LogTracker.Instance;
            MessageReceivedEventArgs[] logList = tracker.LogList.ToArray();
			//add log messages as strings to list: type, message, type, message etc.
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
			//rest of args for commandReceivedEventArgs
            int id = (int)CommandEnum.LogHistoryCommand;
			string requestDirPath = "";
			CommandReceivedEventArgs arg = new CommandReceivedEventArgs(id, data.ToArray(), requestDirPath);
			//send client the log history command and return result
            return server.SendClientCommand(int.Parse(args[1]), arg, out result);
        }
    }
}