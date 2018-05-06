using ImageServiceProgram.Commands;
using ImageServiceProgram.Controller;
using Communication.Commands.Enums;
using ImageServiceProgram.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceProgram.Handlers;
using ImageServiceProgram.Event;
using Communication.Commands;
using System.Net;
using System.Net.Sockets;

namespace ImageServiceProgram.Communication
{
    public class ImageServer
    {
        #region Members
        private IImageController Controller;
        private ILoggingService Logger;
        private IPEndPoint EP;
        private TcpListener Listener;
        private IPAddress IP;
        private int Port;
        private IClientHandler clientHandler;
        private List<TcpClient> clients = new List<TcpClient>();
        #endregion

        #region Properties
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;          // The event that notifies about a new Command being recieved
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"> the controller</param>
        /// <param name="logger"> the logger</param>
        public ImageServer(IImageController controller, ILoggingService logger, int port, IClientHandler ch)
        {
            this.Controller = controller;
            this.Logger = logger;
            this.Port = port;
            this.clientHandler = ch;
            ch.CommandReceivedForHandlers += delegate (object sender, CommandReceivedEventArgs cmdArgs)
            {
                SendHandlersCommand(cmdArgs);
            };
        }

        public void StartServer()
        {
            this.IP = IPAddress.Parse("127.0.0.1");
            EP = new IPEndPoint(IP, Port);
            Listener = new TcpListener(EP);
            Listener.Start();
            Task task = new Task(() =>
            {
                while(true)
                {
                    try
                    {
                        TcpClient client = Listener.AcceptTcpClient();
                        clients.Add(client);
                        clientHandler.HandleClient(client, Logger);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            });
            task.Start();
        }

        public void Stop()
        {
            Listener.Stop();
        }

        /// <summary>
        /// create a handler
        /// </summary>
        /// <param name="directory">the directory the handler will operate on</param>
        public void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoryHandler(Controller, Logger);
            CommandReceived += handler.OnCommandReceived;
            handler.DirectoryClose += onHandlerClose;
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
        /// when server closes
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="args">arguments relevant to server shut down</param>
        public void onHandlerClose(object sender, DirectoryCloseEventArgs args)
        {
            DirectoryHandler handler = (DirectoryHandler)sender;
            handler.DirectoryClose -= onHandlerClose;
            CommandReceived -= handler.OnCommandReceived;
            //ADD: tell all clients that handler closed
        }      
    }
}