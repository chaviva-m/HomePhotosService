using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.CommandInfrastructure
{
	public class CommandEnumName
	{
		/// <summary>
		/// get name of command
		/// </summary>
		/// <param name="commandID">id of command</param>
		/// <returns>name of command</returns>
		public static string CommandName(int commandID)
		{
			switch (commandID)
			{
				case (int)CommandEnum.NewFileCommand:
					return "NewFileCommand";
				case (int)CommandEnum.GetConfigCommand:
					return "GetConfigCommand";
				case (int)CommandEnum.LogHistoryCommand:
					return "LogHistoryCommand";
				case (int)CommandEnum.LogUpdateCommand:
					return "LogUpdateCommand";
				case (int)CommandEnum.CloseDirectoryCommand:
					return "CloseDirectoryCommand";
				case (int)CommandEnum.CloseServerCommand:
					return "CloseServerCommand";
				case (int)CommandEnum.CloseClientCommand:
					return "CloseClientCommand";
				default:
					return "";
			}
		}
	}

	public enum CommandEnum : int
	{
		NewFileCommand,         //add new file to output directory
		GetConfigCommand,       //get settings in app config
		LogHistoryCommand,      //get all logs from start of service
		LogUpdateCommand,       //get most recent log 
		CloseDirectoryCommand,  //close directory
		CloseServerCommand,     //server closed
		CloseClientCommand      //client (GUI) closed
	}
}