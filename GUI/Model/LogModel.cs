using Communication.Commands;
using Communication.Commands.Enums;
using GUI.TcpClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GUI.Model
{
    public class LogModel : ILogModel
    {

        public LogModel()
        {
            ClientChannel clientChannel = ClientChannel.Instance;
            //add methods to client channel's event
            clientChannel.CommandReceived += GetLogHistory;
            clientChannel.CommandReceived += GetLogUpdate;
            //request log history
            Debug.WriteLine("sending command to get log history");
            string[] args = { "" };
            clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.LogHistoryCommand, args, ""));
        }

        private void GetLogHistory(object sender, CommandReceivedEventArgs cmdArgs)
        {
            if (cmdArgs.CommandID == (int)CommandEnum.LogHistoryCommand)
            {
                //iterate array and add logs to LogMessages type, message
                string[] logs = cmdArgs.Args;
                int size = logs.Length;
                for (int i = 0; i < size - 1; i += 2)
                {
                    Log log = new Log(logs[i], logs[i + 1]);
                    AddLog(log);
                }
            }
        }

        private void GetLogUpdate(object sender, CommandReceivedEventArgs cmdArgs)
        {
            if (cmdArgs.CommandID == (int)CommandEnum.LogUpdateCommand)
            {
                //update log
                Log log = new Log(cmdArgs.Args[0], cmdArgs.Args[1]); //reminder fix enum
                AddLog(log);
            }
        }

        private ObservableCollection<Log> logMessages = new ObservableCollection<Log>();
        public ObservableCollection<Log> LogMessages
        {
            get { return logMessages; }
            set { logMessages = value; }
        }

        //add this function in delegate to client's event, change later to private
        public void AddLog(Log log)
        {
            LogMessages.Add(log);
            Debug.WriteLine("added to list: " + log.Type + "message: " + log.Message);
        }

    }
}
