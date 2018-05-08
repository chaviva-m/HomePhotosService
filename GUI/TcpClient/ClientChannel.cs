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

namespace GUI.TcpClient
{
    public sealed class ClientChannel
    {
        /*update server to close client when we exit GUI*/

        //make event that will pass command from server
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;

        private IPEndPoint ep;
        private System.Net.Sockets.TcpClient client;

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
                    Debug.WriteLine("started task " + Task.CurrentId);
                    Debug.WriteLine("will now read commands in infinite lopp");
                    ReadCommands();
                    Debug.WriteLine("exited read commands. finished task " + Task.CurrentId);
                });
                t.Start();
            }
        }

        private bool Connect(IPAddress ip, int port)
        {
            ep = new IPEndPoint(ip, port);
            client = new System.Net.Sockets.TcpClient();
            try
            {
                client.Connect(ep);
                Debug.WriteLine("connected to server. ");
                return true;
            } catch (Exception e)
            {
                Debug.WriteLine("couldn't connect to server. " + e.Message); //CHANGE
                return false;
            }
        }
       
        private void ReadCommands()
        {
            /*add try catch?*/

            NetworkStream stream = client.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            while (true) //use variable to stop the loop when exit GUI?
            {
                try
                {
                    string input = reader.ReadString();
                    CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
                    CommandReceived?.Invoke(this, cmdArgs);
                } catch(Exception e)
                {
                    Debug.WriteLine("client channel, in ReadCommands. Couldn't read from server\n" + e.Message);
                }
            }
            //reader.Dispose();
            /*close client socket*/

            /*using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                while(true) //use variable to stop the loop when exit GUI?
                {
                    string input = reader.ReadLine();
                    CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
                    CommandReceived?.Invoke(this, cmdArgs);
                }
            }*/
        }

        public void SendCommand(CommandReceivedEventArgs cmdArgs)
        {
            Task t = new Task(() =>
            {
                /*try catch?*/
                Debug.WriteLine("started task " + Task.CurrentId);

                NetworkStream stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                string output = JsonConvert.SerializeObject(cmdArgs);

                Debug.WriteLine("sending server\n" + output);
                try
                {
                    writer.Write(output);
                } catch(Exception e)
                {
                    Debug.WriteLine("in client channel, send command. couldn't send message.\n" + e.Message);
                }
                //writer.Dispose();
                /*using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string output = JsonConvert.SerializeObject(cmdArgs);
                    writer.Write(output);
                }*/
                Debug.WriteLine("finished task " + Task.CurrentId);
            });
            t.Start();
        }
    }
}