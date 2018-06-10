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
        public LogModel()
        {
		}

        /// <summary>
        /// if command is LogHistoryCommand, adds all log messages in cmdArgs
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="cmdArgs">cmdArgs.Args = type, message, type, message etc.</param>
        //private void GetLogHistory(object sender, CommandReceivedEventArgs cmdArgs)
		private void GetLogHistory(CommandReceivedEventArgs cmdArgs)
		{
            //if (cmdArgs.CommandID == (int)CommandEnum.LogHistoryCommand)
            //{
                //iterate over array and add logs to LogMessages type, message
                string[] logs = cmdArgs.Args;
                int size = logs.Length;
                for (int i = 0; i < size - 1; i += 2)
                {
                    Log log = new Log(logs[i], logs[i + 1]);
                    AddLog(log);
                }
            //}
        }

        /// <summary>
        /// if command is LogUpdateCommand, adds new log message in cmdArgs to LogMessages
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="cmdArgs">cmdArgs.Args[0]=type, cmdArgs.Args[1]=message</param>
        /*private void GetLogUpdate(object sender, CommandReceivedEventArgs cmdArgs)
        {
            if (cmdArgs.CommandID == (int)CommandEnum.LogUpdateCommand)
            {
                //update log
                Log log = new Log(cmdArgs.Args[0], cmdArgs.Args[1]);
                AddLog(log);
            }
        }*/

        //log messages
        private List<Log> logMessages = new List<Log>();
        //lock
        //private Object thisLock = new Object();
        public List<Log> LogMessages
        {
            get { getLogMessagesFromServer(); return logMessages; }
            set { logMessages = value; }
        }

		private void getLogMessagesFromServer()
		{
			clientChannel clientChannel = clientChannel.Instance;
			//add methods to client channel's event
			//clientChannel.CommandReceived += GetLogHistory;
			//clientChannel.CommandReceived += GetLogUpdate;
			//request log history
			string[] args = { "" };
			clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.LogHistoryCommand, args, ""));
			CommandReceivedEventArgs cmdArgs = clientChannel.ReadCommand();
			if (cmdArgs != null)
			{
				GetLogHistory(cmdArgs);
			}

		}

        /// <summary>
        /// add log to LogMessages
        /// </summary>
        /// <param name="log">the log to add</param>
        private void AddLog(Log log)
        {
            LogMessages.Add(log);
        }

        public void LeaveLogType(string type)
        {
            //lock(thisLock)
            //{
                for (int i = LogMessages.Count - 1; i >= 0; i--)
                {   
                    if (LogMessages[i].Type.CompareTo(type) != 0)
                    {
                        LogMessages.Remove(LogMessages[i]);
                    }
                }
            //}
        }

    }
}
