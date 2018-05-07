using ImageServiceProgram.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Service
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
            get { return logList; }
        }

        public void OnLog(object sender, MessageReceivedEventArgs args)
        {
            logList.Add(args);
        }

        public void ClearLogTracker()
        {
            logList.Clear();
        }
    }
}
