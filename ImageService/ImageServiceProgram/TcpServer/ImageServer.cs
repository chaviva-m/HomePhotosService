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
using System.IO;
using Newtonsoft.Json;
using ImageServiceProgram.Logging.Modal;
using System.Diagnostics;
using System.Threading;

namespace ImageServiceProgram.TcpServer
{
    public class ImageServer : IImageServer
    {
        /*Add function to remove client*/

        #region Members
        private IImageController controller;
        public IImageController Controller
        {
            set { controller = value; }
        }
        private ILoggingService Logger;
        private IPEndPoint EP;
        private TcpListener Listener;
        private IPAddress IP;
        private int Port;
        private IClientHandler clientHandler;
        private Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
        private bool stop;
        private int lastClientID;
        //private static readonly Mutex mutex = new Mutex();
        #endregion

        #region Properties
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;          // The event that notifies about a new Command being recieved
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"> the controller</param>
        /// <param name="logger"> the logger</param>
        public ImageServer(ILoggingService logger, int port, IClientHandler ch)
        {
            this.Logger = logger;
            Logger.MessageRecieved += SendClientsLog;
            this.Port = port;
            this.clientHandler = ch;
            ch.CommandReceivedForHandlers += delegate (object sender, CommandReceivedEventArgs cmdArgs)
            {
                SendHandlersCommand(cmdArgs);
            };
        }

        public void StartServer()
        {
            stop = false;
            lastClientID = 0;
            this.IP = IPAddress.Parse("127.0.0.1");
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
                        Debug.WriteLine("got a client");
                        lastClientID += 1;
                        clients.Add(lastClientID, client);
                        clientHandler.HandleClient(client, lastClientID, Logger);//, mutex);
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
            stop = true;
            Listener.Stop();
            //close all clients
            foreach (TcpClient client in clients.Values)
            {
                client.Close();
            }
        }

        public void SendClientsLog(object sender, MessageReceivedEventArgs message)
        {
            string[] args = { message.Status.ToString(), message.Message };
            CommandReceivedEventArgs cmdArgs = new CommandReceivedEventArgs((int)CommandEnum.LogUpdateCommand, args, "");
            bool result;
            foreach (int id in clients.Keys)
            {
                SendClientCommand(id, cmdArgs, out result);
            }
        }

        public string SendClientCommand(int id, CommandReceivedEventArgs Args, out bool result)
        {
            //task??
            string msg;

                NetworkStream stream = clients[id].GetStream();
                BinaryWriter writer = new BinaryWriter(stream);

                try
                {
                    string output = JsonConvert.SerializeObject(Args);
                    Debug.WriteLine("want to send client\n" + output);
                    //mutex.WaitOne();
                    writer.Write(output);
                    //mutex.ReleaseMutex();
                    Debug.WriteLine("sent client the output");
                    result = true;
                    msg = "Sent client command: " + Args.CommandID;
                }
                catch (Exception e)
                {
                    
                    result = false;
                    //maybe indicates that we should remove client from list...?
                    msg = "Couldn't send client command " + Args.CommandID + ". " + e.Message;
                    //or: msg = "Client disconnected from server.";
                } finally
                {
                //mutex.ReleaseMutex();
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
            string[] arr = { "" };
            //ADD: tell all clients that handler closed
            CommandReceivedEventArgs cmdArgs = new CommandReceivedEventArgs((int)CommandEnum.CloseCommand, arr, args.DirectoryPath);
            bool result;
            foreach (int id in clients.Keys)
            {
                SendClientCommand(id, cmdArgs, out result);
            }
        }      
    }
}

