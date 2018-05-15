using Communication.Commands;
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

namespace ImageServiceProgram.TcpServer
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

        public void HandleClient(TcpClient client, int clientID, ILoggingService logger)//, Mutex mutex)
        {
            Task task = new Task(() =>
            {
                /*need to close stream*/
                string input = "";
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                //read command input from client
                try
                {
                    //mutex.WaitOne();
                    input = reader.ReadString();
                    
                } catch(Exception e)
                {
                    //mutex.ReleaseMutex();
                    Debug.WriteLine("in Client handler, handle client. read line failed\n" + e.Message);
                    return;
                } finally
                {
                    //mutex.ReleaseMutex();
                }
                //reader.Dispose();
                CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
                Debug.WriteLine("got fom client\n" + cmdArgs);
                //add client (serialized) to end of args
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
            });
            task.Start();
        }
    }
}