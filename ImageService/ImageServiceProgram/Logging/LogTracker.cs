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
				string msg = " I once wrote an incredible story. Everyone wanted to read it. It became a best seller. I got rich. It was amazing. I will be forever grateful to those who stood by my side.";
				List<MessageReceivedEventArgs> temp = new List<MessageReceivedEventArgs>();
				for(int i = 0; i < 10; i++)
				{
					temp.Add(new MessageReceivedEventArgs(MessageTypeEnum.INFO, "num: " + i + msg));
				}
				for (int i = 0; i < 10; i++)
				{
					temp.Add(new MessageReceivedEventArgs(MessageTypeEnum.FAIL, "num: " + i));
				}
				for (int i = 0; i < 10; i++)
				{
					temp.Add(new MessageReceivedEventArgs(MessageTypeEnum.WARNING, "num: " + i));
				}
				return temp;
				//return logList;
			}
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
