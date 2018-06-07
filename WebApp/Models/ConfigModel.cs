using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.CommandInfrastructure;

namespace WebApp.Models
{
	public class ConfigModel
	{
		private string outputDirectory;
		public string OutputDirectory { get { return outputDirectory; } set { outputDirectory = value; } }
		private string sourceName;
		public string SourceName { get { return sourceName; } private set { sourceName = value; } }
		private string logName;
		public string LogName { get { return logName; } private set { logName = value; } }
		private string thumbnailSize;
		public string ThumbnailSize { get { return thumbnailSize; } private set { thumbnailSize = value; } }
		private List<string> directories = new List<string>();
		public List<string> Directories { get { return directories; } private set { directories = value; } }
		private string dirToRemove;
		public string DirToRemove { get { return dirToRemove; } set { dirToRemove = value; } }

		public ConfigModel()
		{
			ClientChannel clientChannel = ClientChannel.Instance;
			//add methods to client channel's event
			clientChannel.CommandReceived += GetAppConfig;
			clientChannel.CommandReceived += DeleteDir;
			//request app config settings
			string[] args = { "" };
			clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, args, ""));
			/*CommandReceivedEventArgs cmdArgs = clientChannel.ReadCommand();
			if (cmdArgs != null)
			{
				GetAppConfig(cmdArgs);
			}*/
		}

		/// <summary>
		/// if command is GetConfigCommand, sets all values of settings according to cmdArgs
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="cmdArgs">commmand args</param>
		private void GetAppConfig(object sender, CommandReceivedEventArgs cmdArgs)
		//private void GetAppConfig(CommandReceivedEventArgs cmdArgs)
		{
			if (cmdArgs.CommandID == (int)CommandEnum.GetConfigCommand)
			{
				//set all properties to values in args from client channel
				OutputDirectory = cmdArgs.Args[0];
				SourceName = cmdArgs.Args[1];
				LogName = cmdArgs.Args[2];
				ThumbnailSize = cmdArgs.Args[3];
				for (int i = 4; i < cmdArgs.Args.Length; i++)
				{
					directories.Add(cmdArgs.Args[i]);
				}
			}
		}

		/// <summary>
		/// if command is CloseDirectoryCommand, delets relevant directory from directories
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="cmdArgs">commmand args</param>
		private void DeleteDir(object sender, CommandReceivedEventArgs cmdArgs)
		//private bool DeleteDir(CommandReceivedEventArgs cmdArgs)
		{
			if (cmdArgs.CommandID == (int)CommandEnum.CloseDirectoryCommand)
			{
				directories.Remove(cmdArgs.RequestDirPath);
				//return true;
			}
			//return false;
		}

		public void DeleteDirRequest()
		//public string DeleteDirRequest(out bool result)
		{
			ClientChannel channel = ClientChannel.Instance;
			string[] args = { "" };
			channel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.CloseDirectoryCommand, args, DirToRemove));
			/*CommandReceivedEventArgs cmdArgs = channel.ReadCommand();
			if (cmdArgs == null)
			{
				result = false;
				return "Couldn't read from server\n";
			} else
			{
				result = DeleteDir(cmdArgs);
				if (result == false)
				{
					return "Couldn't remove directory\n";
				} else
				{
					return "Successfully removed directory\n";
				}
			}*/
		}
	}
}