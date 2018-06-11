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

		/// <summary>
		/// config model constructor. get info frm server.
		/// </summary>
		public ConfigModel()
		{
			CommandReceivedEventArgs cmdArgs;
			clientChannel clientChannel = clientChannel.Instance;
			//request app config settings
			string[] args = { "" };
			clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, args, ""));
			cmdArgs = clientChannel.ReadCommand();
			if (cmdArgs != null)
			{
				GetAppConfig(cmdArgs);
			}
		}

		/// <summary>
		/// sets all values of settings according to cmdArgs
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="cmdArgs">commmand args</param>
		//private void GetAppConfig(object sender, CommandReceivedEventArgs cmdArgs)
		private void GetAppConfig(CommandReceivedEventArgs cmdArgs)
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

		/// <summary>
		/// delete relevant directory from directories
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="cmdArgs">commmand args</param>
		private void DeleteDir(CommandReceivedEventArgs cmdArgs)
		{
			directories.Remove(cmdArgs.RequestDirPath);
		}

		/// <summary>
		/// send server command to delete directory.
		/// </summary>
		/// <param name="result">set true on success, false on failure</param>
		/// <returns>return string describing result of deletion</returns>
		public string DeleteDirRequest(out bool result)
		{
			clientChannel channel = clientChannel.Instance;
			string[] args = { "" };
			channel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.CloseDirectoryCommand, args, DirToRemove));
			CommandReceivedEventArgs cmdArgs = channel.ReadCommand();
			if (cmdArgs == null)
			{
				result = false;
				return "Couldn't read from server\n";
			} else
			{
				DeleteDir(cmdArgs);
				result = true;
				return "Successfully removed directory\n";
			}
		}
	}
}