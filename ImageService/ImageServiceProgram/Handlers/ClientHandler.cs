using CommandInfrastructure.Commands;
using ImageServiceProgram.Controller;
using ImageServiceProgram.Logging;
using ImageServiceProgram.Logging.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageServiceProgram.Handlers
{
    public class ClientHandler : IClientHandler
    {
        private IImageController controller;
        public IImageController Controller
        {
            set { controller = value; }
        }
        public event EventHandler<CommandReceivedEventArgs> CommandReceivedForHandlers;

        public ClientHandler() { }

		/// <summary>
		/// read commands from client
		/// </summary>
		/// <param name="client">the tcp client to handle</param>
		/// <param name="clientID">the client's id</param>
		/// <param name="logger">ILoggingService</param>
		public void HandleClient(TcpClient client, int clientID, ILoggingService logger)
        {
            Task task = new Task(() =>
            {              
                string input = "";
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                while (client.Connected)
                {
					//read command input from client
					try
					{
						input = reader.ReadString();
						ExecuteCommand(input, clientID, logger);
					}
					catch (Exception)
					{
						return;
					}
                }
            });
            task.Start();
        }

		/// <summary>
		/// execute client's command
		/// </summary>
		/// <param name="command">the command</param>
		/// <param name="clientID">the id of the client</param>
		/// <param name="logger">ILoggingService</param>
		private void ExecuteCommand(string command, int clientID, ILoggingService logger)
		{
			CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(command);
			//add client id to end of args
			string[] argsArr = cmdArgs.Args;
			Array.Resize(ref argsArr, cmdArgs.Args.Length + 1);
			argsArr[argsArr.Length - 1] = clientID.ToString();
			cmdArgs.Args = argsArr;

			//execute command
			bool result;
			string msg;
			if (cmdArgs.RequestDirPath != "")
			{
				CommandReceivedForHandlers?.Invoke(this, cmdArgs);
			}
			else
			{
				msg = controller.ExecuteCommand(cmdArgs.CommandID, cmdArgs.Args, out result);
				MessageTypeEnum mte;
				//find out if command succeeded or not in order to inform logger
				if (result == false)
				{
					mte = MessageTypeEnum.FAIL;
				}
				else
				{
					mte = MessageTypeEnum.INFO;
				}
				//inform logger
				logger.Log(msg, mte);
			}
		}
    }
}