using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WebApp.CommandInfrastructure;

namespace WebApp.Models
{
	public sealed class ClientChannel
	{
		//event that will be invoked when client receives command from server
		//public event EventHandler<CommandReceivedEventArgs> CommandReceived;

		public static readonly IPAddress IP = IPAddress.Parse("127.0.0.1");
		public static readonly int Port = 8000;

		private IPEndPoint ep;
		private TcpClient client;
		private NetworkStream stream;
		private BinaryReader reader;
		private BinaryWriter writer;
		//private bool stop;

		private Object thisLock = new Object();

		private bool isConnected = false;
		public bool IsConnected { get { return isConnected; } private set { isConnected = value; } }


		private static readonly ClientChannel instance = new ClientChannel();
		public static ClientChannel Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// private constructor of singleton. Only called once.
		/// connects to server.
		/// </summary>
		private ClientChannel()
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
				//read commands
				/*Task t = new Task(() =>
				{
					ReadCommands();
				});
				t.Start();*/
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

		/// <summary>
		/// read commands from server in infinite loop as long as connected to server
		/// </summary>
		/*private void ReadCommands()
		{
			stop = false;
			reader = new BinaryReader(stream);
			while (!stop)
			{
				try
				{
					string input = reader.ReadString();
					//execute input command
					//DispatchCommand(input);
					//see if this works
					CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
					CommandReceived?.Invoke(this, cmdArgs);
				}
				catch (Exception)
				{
					//connection with server was disconnected
					OnStop();
				}
			}
		}*/

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
				return null; //should I return null?
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
		/// dispatch command to UI thread and execute there.
		/// </summary>
		/// <param name="input">the command to dispatch</param>
		/*private void DispatchCommand(string input)
		{
			//the data in the view is created on UI thread, therefore we can only modify it from the UI thread.
			//we put the delegate on UI Dispatcher and that will do work for us delegating it to UI thread.
			App.Current.Dispatcher.Invoke((Action)delegate
			{
				CommandReceivedEventArgs cmdArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(input);
				CommandReceived?.Invoke(this, cmdArgs);
			});
		}*/

		/// <summary>
		/// stop connection, close streams
		/// </summary>
		private void OnStop()
		{
			//stop = true;
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