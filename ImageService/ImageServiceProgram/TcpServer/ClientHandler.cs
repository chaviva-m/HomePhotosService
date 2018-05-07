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
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.TcpServer
{
    public class ClientHandler : IClientHandler
    {
        private IImageController Controller;
        public event EventHandler<CommandReceivedEventArgs> CommandReceivedForHandlers;

        public ClientHandler(IImageController controller)
        {
            Controller = controller;
        }

        public void HandleClient(TcpClient client, ILoggingService logger)
        {
            Task task = new Task(() =>
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    //read command input from client
                    string input = reader.ReadLine();
                    CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
                    Debug.WriteLine("got fom client\n" + cmdArgs);
                    //add client (serialized) to end of args
                    string[] argsArr = cmdArgs.Args;
                    Array.Resize(ref argsArr, cmdArgs.Args.Length + 1);
                    argsArr[argsArr.Length - 1] = JsonConvert.SerializeObject(client);
                    cmdArgs.Args = argsArr;

                    //execute command
                    bool result;
                    string msg;
                    if (cmdArgs.RequestDirPath != "")
                    {
                        CommandReceivedForHandlers?.Invoke(this, cmdArgs);
                    } else
                    {
                        msg = Controller.ExecuteCommand(cmdArgs.CommandID, cmdArgs.Args, out result);
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
            });
            task.Start();
        }
    }
}