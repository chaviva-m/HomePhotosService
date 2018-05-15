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
                    //mutex.ReleaseMutex();
                    result = false;
                    //maybe indicates that we should remove client from list...?
                    msg = "Couldn't send client command " + Args.CommandID + ". " + e.Message;
                    //or: msg = "Client disconnected from server.";
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

    //PASS CommandReceivedEventArgs to new project and make serialiazable
    //open thread always listening to clients(connect/listen)
    //if client connects-add to client list and open new thread to carry out client request (CommandReceivedEventArgs)
    //for all commands received by server: check if has path- if yes:  invoke sendHandlerCommand (pass to handlers) , if not - to controller via new method sendControllerCommand
    //make SendCLientLog(object sender, messageReceivedEventArgs mr) translates it into CommandReceivedEventArgs and send to generic SendClientCommand(int clientid,CommandReceivedEventArgs cr) .
    //service makes dictionary with:
    //new file commandCommand(ImageModal)

    //logCommand (hold server) -gets  list of log history from the singelton which holds a list of
    //MessageReceivedEventArgs and iterates over list sending each one to sendClientLog by invoking 
    //SendCLientLog from server (extracts client id from the end of the list)
    //
    //SendCLientsLog(object sender, messageReceivedEventArgs mr) subscribes to singelton, iterates over all clients and sends mr
    //SendCLientLog(object sender, messageReceivedEventArgs mr, int clientid)

    //when server received command it will add client id as last elemnt in args[] by using:
    //Array.Resize(ref array, newsize);
    //array[newsize - 1] = "newvalue"
    //app config and logComand will remember this and extract client from the end of args and send data to
    //relevant client only

    //(singelton appConfigSingelton which holds all data from appConfig in shape of CommandReceivedEventArgs)
    //config command gets data from singleton and hold server -invokes sendClienCommand in server
    //(get the id from args [] - it will be the only string in the array