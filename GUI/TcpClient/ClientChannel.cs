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
using System.Threading;
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
        private bool stop;
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        public static readonly IPAddress IP  = IPAddress.Parse("127.0.0.1");
        public static readonly int Port = 8000;


        private static readonly ClientChannel instance = new ClientChannel();
        public static ClientChannel Instance
        {
            get { return instance; }
        }

        private ClientChannel()
        {        
            bool connect = Connect(IP, Port);
            if (!connect)
            {
                //do something if can't connect
            } else
            {
                //open stream
                stream = client.GetStream();
                writer = new BinaryWriter(stream);
                //read commands
                Task t = new Task(() =>
                {
                    Debug.WriteLine("will now read commands in infinite lopp");
                    ReadCommands();
                    Debug.WriteLine("exited read command");
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
                Debug.WriteLine("couldn't connect to server. " + e.Message);
                return false;
            }
        }
       
        private void ReadCommands()
        {
            /*add try catch?*/
            stop = false;
            reader = new BinaryReader(stream);
            while (!stop) //use variable to stop the loop when server closes?
            {
                try
                {
                    string input = reader.ReadString();
                    DispatchCommand(input);
                } catch(Exception e)
                {
                    OnStop();
                    Debug.WriteLine("client channel, in ReadCommands. Couldn't read from server\n" + e.Message);
                }
            }
        }

        public void SendCommand(CommandReceivedEventArgs cmdArgs)
        {
            Task t = new Task(() =>
            {
                Debug.WriteLine("Client channel: SendCommand. Started task " + Task.CurrentId);

                string output = JsonConvert.SerializeObject(cmdArgs);
                Debug.WriteLine("sending server\n" + output);
                try
                {
                    writer.Write(output);
                } catch(Exception e)
                {
                    Debug.WriteLine("in client channel, send command. couldn't send message.\n" + e.Message);
                }
                Debug.WriteLine("Exiting client channel, send command. finished task " + Task.CurrentId);
            });
            t.Start();
        }

        private void DispatchCommand(string input)
        {
            //the data in the view is created on UI thread, therefore we can only modify it from the UI thread.
            //we put the delegate on UI Dispatcher and that will do work for us delegating it to UI thread.
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
                CommandReceived?.Invoke(this, cmdArgs);
            });
        }

        private void OnStop()
        {
            stop = true;
            reader.Close();
            writer.Close();
            stream.Close();
        }
    }
}