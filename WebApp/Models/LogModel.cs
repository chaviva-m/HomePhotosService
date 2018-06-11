using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.CommandInfrastructure;
using WebApp.Models;

namespace WebApp.Models
{
    public class LogModel
    {
		/// <summary>
		/// constructor
		/// </summary>
		public LogModel() { }

		/// <summary>
		/// adds all log messages in cmdArgs
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="cmdArgs">cmdArgs.Args = type, message, type, message etc.</param>
		private void GetLogHistory(CommandReceivedEventArgs cmdArgs)
		{
			//iterate over array and add logs to LogMessages type, message
			string[] logs = cmdArgs.Args;
			int size = logs.Length;
			List<Log> logsList = new List<Log>();
			for (int i = 0; i < size - 1; i += 2)
			{
				Log log = new Log(logs[i], logs[i + 1]);
				logsList.Add(log);
			}
			LogMessages = logsList;
		}

		/// <summary>
		/// request log history from server.
		/// </summary>
		private void getLogMessagesFromServer()
		{
			clientChannel clientChannel = clientChannel.Instance;
			//request log history
			string[] args = { "" };
			clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.LogHistoryCommand, args, ""));
			CommandReceivedEventArgs cmdArgs = clientChannel.ReadCommand();
			if (cmdArgs != null)
			{
				GetLogHistory(cmdArgs);
			}
		}

		//log messages
		private List<Log> logMessages = new List<Log>();
		public List<Log> LogMessages
		{
			get { getLogMessagesFromServer(); return logMessages; }
			set { logMessages = value; }
		}
	}
}
