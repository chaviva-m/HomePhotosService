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
        /// if command is LogHistoryCommand, adds all log messages in cmdArgs
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

        public void Refresh()
        {
            getLogMessagesFromServer();
        }
        //refresh boolean
        private bool refreshed;
        public bool Refreshed { get { return refreshed; } set { refreshed = value; } }
        // log filter
        private string filter;
        public string Filter { get { return filter; } set { filter = value; } }
        //log messages
        private List<Log> logMessages = new List<Log>();
        public List<Log> LogMessages
        {
            get { return logMessages; }
            set { logMessages = value; }
        }

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

        public void LeaveLogType(string type)
		{
			List<Log> logs = LogMessages;
			for (int i = logs.Count - 1; i >= 0; i--)
			{
				if (logs[i].Type.CompareTo(type) != 0)
				{
					logs.Remove(logs[i]);
				}
			}
			LogMessages = logs;
        }

    }
}
