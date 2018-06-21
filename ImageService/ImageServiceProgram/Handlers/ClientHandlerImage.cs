using CommandInfrastructure.Commands;
using CommandInfrastructure.Commands.Enums;
using ImageServiceProgram.Controller;
using ImageServiceProgram.Handlers;
using ImageServiceProgram.Logging;
using ImageServiceProgram.Logging.Modal;
using ImageServiceProgram.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Handlers
{
	class ClientHandlerImage : IClientHandler
	{
		private IImageController controller;
		public IImageController Controller { set { controller = value; } }

		public event EventHandler<CommandReceivedEventArgs> CommandReceivedForHandlers;

		public ClientHandlerImage() { }

		/// <summary>
		/// read commands from client
		/// </summary>
		/// <param name="client">the tcp client to handle</param>
		/// <param name="clientID">the client's id</param>
		/// <param name="logger">ILoggingService</param>
		public void HandleClient(TcpClient client, int clientID, ILoggingService logger)
		{
			Task task = new Task(() =>
			{
				byte[] imgBytes;
				string sizeStr = "";
                int size;
				string imgName;
                byte[] nameByte;
                int nameSize;
				NetworkStream stream = client.GetStream();
				BinaryReader reader = new BinaryReader(stream);
				while (client.Connected)
				{
					//read command input from client
					try
					{
                        size = reader.ReadInt32();
                        //sizeStr = reader.ReadString();
                        Debug.WriteLine("get size");
                        //size = Int32.Parse(sizeStr);
						imgBytes = reader.ReadBytes(size);
                        Debug.WriteLine("get imgBytes");
                        nameSize = reader.ReadInt32();
                        nameByte = reader.ReadBytes(nameSize);
                        imgName = System.Text.Encoding.UTF8.GetString(nameByte);
                        Debug.WriteLine("get name");
						ExecuteCommand(imgBytes, imgName, clientID, logger);
					}
					catch (Exception e)
					{
						return;
					}
				}
			});
			task.Start();
		}

		private void ExecuteCommand(byte[] image, string imgName, int clientID, ILoggingService logger)
		{

			/*string imageStr;
			try
			{
				imageStr = Convert.ToBase64String(image);
                Debug.WriteLine("converting bytes to image");
            } catch (Exception)
			{
				logger.Log("could not save picture because convert to base 64 string failed.", MessageTypeEnum.FAIL);
				return;
			}*/
			string handler = AppConfigData.Instance.Directories[0];
            //string[] args = { imageStr, imgName, handler, clientID.ToString() };
            //CommandReceivedEventArgs cmdArgs = new CommandReceivedEventArgs((int)CommandEnum.SaveFileCommand, args, handler);
            //send command to handler
            //CommandReceivedForHandlers?.Invoke(this, cmdArgs);
            Image image2;
            string name;
            ImageFormat format;

            //save byte array as image in handler
            using (MemoryStream mStream = new MemoryStream(image))
            {
                image2 = Image.FromStream(mStream);
                name = imgName;
                try
                {
                    format = GetImageFormat(name);
                    image2.Save(Path.Combine(handler, name), format);
                    Debug.WriteLine("successfully saved image: " + name);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }
        private ImageFormat GetImageFormat(string name)
        {
            string ext = Path.GetExtension(name);
            if (string.IsNullOrEmpty(ext))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", name));

            switch (ext.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
