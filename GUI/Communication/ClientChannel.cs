using Communication.Commands;
using Communication.Commands.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Communication
{
    public sealed class ClientChannel
    {
        /*update server to close client when we exit GUI*/

        //make event that will pass command from server
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;

        private IPEndPoint ep;
        private TcpClient client;

        private static readonly ClientChannel instance = new ClientChannel();
        public static ClientChannel Instance
        {
            get { return instance; }
        }

        private ClientChannel()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");    //how does he know this?
            int port = 8000;                                //how does he know this?
            bool connect = Connect(ip, port);
            if (!connect)
            {
                //do something if can't connect
            } else
            {
                Task t = new Task(() =>
                {
                    ReadCommands();
                });
                t.Start();
            }
        }

        private bool Connect(IPAddress ip, int port)
        {
            ep = new IPEndPoint(ip, port);
            client = new TcpClient();
            try
            {
                client.Connect(ep);
                return true;
            } catch (Exception e)
            {
                Debug.WriteLine("couldn't connect to server. " + e.Message); //CHANGE
                return false;
            }
        }
       
        private void ReadCommands()
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                while(true) //use variable to stop the loop when exit GUI?
                {
                    string input = reader.ReadLine();
                    CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
                    CommandReceived?.Invoke(this, cmdArgs);
                }
            }
        }

        public void SendCommand(CommandReceivedEventArgs cmdArgs)
        {
            //might not work
            using (NetworkStream stream = client.GetStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string output = JsonConvert.SerializeObject(cmdArgs);
                writer.Write(output);
            }
        }
    }
}