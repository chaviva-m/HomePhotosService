using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WebApp.CommandInfrastructure;

namespace WebApp.Models
{
	public sealed class clientChannel
	{
		public static readonly IPAddress IP = IPAddress.Parse("127.0.0.1");
		public static readonly int Port = 8000;

		private IPEndPoint ep;
		private TcpClient client;
		private NetworkStream stream;
		private BinaryReader reader;
		private BinaryWriter writer;

		private Object thisLock = new Object();

		private bool isConnected = false;
		public bool IsConnected { get { return isConnected; } private set { isConnected = value; } }


		private static readonly clientChannel instance = new clientChannel();
		public static clientChannel Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// private constructor of singleton. Only called once.
		/// connects to server.
		/// </summary>
		private clientChannel()
		{
			bool connect = Connect();
			if (!connect)
			{
				IsConnected = false;
			}
			else
			{
				IsConnected = true;
				//open stream
				stream = client.GetStream();
				writer = new BinaryWriter(stream);
				reader = new BinaryReader(stream);
			}
		}

		/// <summary>
		/// connect to server
		/// </summary>
		/// <returns>true if connection succeeded, otherwise false</returns>
		private bool Connect()
		{
			ep = new IPEndPoint(IP, Port);
			client = new System.Net.Sockets.TcpClient();
			try
			{
				client.Connect(ep);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	
		public CommandReceivedEventArgs ReadCommand()
		{
			try
			{
				string input = reader.ReadString();
				CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
				return cmdArgs;
			}
			catch (Exception)
			{
				//connection with server was disconnected
				OnStop();
				return null;
			}
		}

		/// <summary>
		/// send server command
		/// </summary>
		/// <param name="cmdArgs">the command to send to server</param>
		public void SendCommand(CommandReceivedEventArgs cmdArgs)
		{
			Task t = new Task(() =>
			{
				string output = JsonConvert.SerializeObject(cmdArgs);
				lock (thisLock)
				{
					try
					{
						writer.Write(output);
					}
					catch (Exception)
					{
						//connection with server was disconnected
						OnStop();
					}
				}
			});
			t.Start();
		}

		/// <summary>
		/// stop connection, close streams
		/// </summary>
		private void OnStop()
		{
			if (isConnected == true)
			{
				isConnected = false;
				reader.Close();
				writer.Close();
				stream.Close();
			}
		}
	}
}