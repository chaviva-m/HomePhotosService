using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.CommandInfrastructure
{
	public class CommandReceivedEventArgs
	{
		public int CommandID { get; set; }      // The Command ID
		public string[] Args { get; set; }
		public string RequestDirPath { get; set; }  // The Request Directory

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="id">command id</param>
		/// <param name="args">command arguments</param>
		/// <param name="path">directory path</param>
		public CommandReceivedEventArgs(int id, string[] args, string path)
		{
			CommandID = id;
			Args = args;
			RequestDirPath = path;
		}
	}
}