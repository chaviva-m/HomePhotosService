using GUI.communication;
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
    class LogModel : ILogModel
    {

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
