using ImageServiceProgram.Commands;
using ImageServiceProgram.Controller;
using CommandInfrastructure.Commands.Enums;
using ImageServiceProgram.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceProgram.Handlers;
using ImageServiceProgram.Event;
using CommandInfrastructure.Commands;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using ImageServiceProgram.Logging.Modal;
using System.Diagnostics;
using System.Threading;
using ImageServiceProgram.Service;

namespace ImageServiceProgram.TcpServer
{
    public class ImageServer : IImageServer
    {
        #region Members
        private IImageController controller;
        public IImageController Controller {  set { controller = value; }  }
        private ILoggingService Logger;
        private IPEndPoint EP;
        private TcpListener Listener;
        private IPAddress IP;
        private int Port;
        private IClientHandler clientHandler;
        private Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
		private int lastClientID;
		private bool stop;
		#endregion

		#region Properties
		//The event that notifies about a new Command being recieved
		public event EventHandler<CommandReceivedEventArgs> CommandReceived;
		#endregion

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="controller"> the controller</param>
		/// <param name="logger"> the logger</param>
		/// <param name="ch">IClientHandler</param>
		public ImageServer(ILoggingService logger, int port, IClientHandler ch)
        {
            this.Logger = logger;
            Logger.MessageRecieved += SendClientsLog;
            this.Port = port;
			this.IP = IPAddress.Parse("127.0.0.1");
			this.clientHandler = ch;
            ch.CommandReceivedForHandlers += delegate (object sender, CommandReceivedEventArgs cmdArgs)
            {
                SendHandlersCommand(cmdArgs);
            };
        }

		/// <summary>
		/// start server
		/// listen to clients in loop until server is stopped
		/// </summary>
        public void StartServer()
        {
            stop = false;
            lastClientID = 0;
            EP = new IPEndPoint(IP, Port);
            Listener = new TcpListener(EP);
            Listener.Start();
            Task task = new Task(() =>
            {
                while(!stop)
                {
                    try
                    {
                        TcpClient client = Listener.AcceptTcpClient();
                        lastClientID += 1;
                        clients.Add(lastClientID, client);
                        clientHandler.HandleClient(client, lastClientID, Logger);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            });
            task.Start();
        }

		/// <summary>
		/// stop server
		/// </summary>
        public void Stop()
        {
            stop = true;
            Listener.Stop();
			//close all clients
			foreach (TcpClient client in clients.Values)
            {
                client.Close();
            }
			clients.Clear();
        }

        public void SendClientsLog(object sender, MessageReceivedEventArgs message)
        {
            string[] args = { message.Status.ToString(), message.Message };
            CommandReceivedEventArgs cmdArgs = new CommandReceivedEventArgs((int)CommandEnum.LogUpdateCommand, args, "");
            bool result;
			Dictionary<int, TcpClient> clientsCopy = new Dictionary<int, TcpClient>(clients);
			foreach (int id in clientsCopy.Keys)
            {

                SendClientCommand(id, cmdArgs, out result);
            }
        }

		/// <summary>
		/// send client a command
		/// </summary>
		/// <param name="id">client id</param>
		/// <param name="Args">command to send client</param>
		/// <param name="result">result of action</param>
		/// <returns>return string indicating if command was successful</returns>
        public string SendClientCommand(int id, CommandReceivedEventArgs Args, out bool result)
        {
            string msg;

			//check if client disconnected
			if(!clients[id].Connected)
			{
				clients.Remove(id);
				result = false;
				return "Client disconnected from server.";
			}
			lock (thisLock)
			{

				NetworkStream stream = clients[id].GetStream();
				BinaryWriter writer = new BinaryWriter(stream);

				//send client the command
				try
				{
					string output = JsonConvert.SerializeObject(Args);
					writer.Write(output);
					result = true;
					msg = "Sent client command: " + Args.CommandID;
				}
				catch (Exception)
				{
					//client disconnected
					clients.Remove(id);
					result = false;
					msg = "Client disconnected from server.";
				}
			}
			return msg;
        }

        /// <summary>
        /// create a handler
        /// </summary>
        /// <param name="directory">the directory the handler will operate on</param>
        public void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoryHandler(controller, Logger);
            CommandReceived += handler.OnCommandReceived;
            handler.DirectoryClose += onHandlerClose;
			handler.DirectoryClose += AppConfigData.Instance.DeleteDirectory;
			handler.StartHandleDirectory(directory);
        }

        /// <summary>
        /// send a command to all handlers
        /// </summary>
        /// <param name="commandArgs">command details</param>
        public void SendHandlersCommand(CommandReceivedEventArgs commandArgs)               
        {
            CommandReceived?.Invoke(this, commandArgs);
        }

		/// <summary>
		/// handle directory closing
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="args">args.DirectoryPath is directory to close</param>
		public void onHandlerClose(object sender, DirectoryCloseEventArgs args)
        {
            DirectoryHandler handler = (DirectoryHandler)sender;
            handler.DirectoryClose -= onHandlerClose;
            CommandReceived -= handler.OnCommandReceived;
            string[] arr = { "" };
            //inform all clients that handler closed
            CommandReceivedEventArgs cmdArgs = new CommandReceivedEventArgs((int)CommandEnum.CloseDirectoryCommand, arr, args.DirectoryPath);
            bool result;
			Dictionary<int, TcpClient> clientsCopy = new Dictionary<int, TcpClient>(clients);
			foreach (int id in clientsCopy.Keys)
            {
                SendClientCommand(id, cmdArgs, out result);
            }
        }      
    }
}