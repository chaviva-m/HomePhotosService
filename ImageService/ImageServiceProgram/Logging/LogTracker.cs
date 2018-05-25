using ImageServiceProgram.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Logging
{
    public sealed class LogTracker
    {
        private static readonly LogTracker instance = new LogTracker();
        public static LogTracker Instance
        {
            get { return instance; }
        }

        private List<MessageReceivedEventArgs> logList = new List<MessageReceivedEventArgs>();
        public List<MessageReceivedEventArgs> LogList
        {
            get
			{
				return logList;
			}
        }

		/// <summary>
		/// add log to list
		/// </summary>
		/// <param name="sender">the object sender</param>
		/// <param name="args">the log to add</param>
        public void OnLog(object sender, MessageReceivedEventArgs args)
        {
            logList.Add(args);
        }

		/// <summary>
		/// clear log list
		/// </summary>
        public void ClearLogTracker()
        {
            logList.Clear();
        }
    }
}