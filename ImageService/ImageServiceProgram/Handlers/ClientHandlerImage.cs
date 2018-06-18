using CommandInfrastructure.Commands;
using CommandInfrastructure.Commands.Enums;
using ImageServiceProgram.Controller;
using ImageServiceProgram.Handlers;
using ImageServiceProgram.Logging;
using ImageServiceProgram.Logging.Modal;
using ImageServiceProgram.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Handlers
{
	class ClientHandlerImage : IClientHandler
	{
		private IImageController controller;
		public IImageController Controller { set { controller = value; } }

		public event EventHandler<CommandReceivedEventArgs> CommandReceivedForHandlers;

		public ClientHandlerImage() { }

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
				byte[] imgBytes;
				string sizeStr = "";
				int size;
				string imgName;
				NetworkStream stream = client.GetStream();
				BinaryReader reader = new BinaryReader(stream);
				while (client.Connected)
				{
					//read command input from client
					try
					{
						sizeStr = reader.ReadString();
						size = Int32.Parse(sizeStr);
						imgBytes = reader.ReadBytes(size);
						imgName = reader.ReadString();
						ExecuteCommand(imgBytes, imgName, clientID, logger);
					}
					catch (Exception)
					{
						return;
					}
				}
			});
			task.Start();
		}

		private void ExecuteCommand(byte[] image, string imgName, int clientID, ILoggingService logger)
		{
			string imageStr;
			try
			{
				imageStr = Convert.ToBase64String(image);
			} catch (Exception)
			{
				logger.Log("could not save picture because convert to base 64 string failed.", MessageTypeEnum.FAIL);
				return;
			}
			string handler = AppConfigData.Instance.Directories[0];
			string[] args = { imageStr, imgName, handler, clientID.ToString() };
			CommandReceivedEventArgs cmdArgs = new CommandReceivedEventArgs((int)CommandEnum.SaveFileCommand, args, handler);
			//send command to handler
			CommandReceivedForHandlers?.Invoke(this, cmdArgs);
		}
	}
}
