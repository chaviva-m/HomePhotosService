using GUI.Model;
using GUI.ViewModel;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GUI.communication;

namespace GUI.ViewModel
{
    public class LogViewModel
    {

        private ILogModel logModel;
        public ILogModel LogModel { get; set; }
        public ObservableCollection<Log> VM_LogMessages { get { return logModel.LogMessages; } }
        public LogViewModel(ILogModel model)
        {
            logModel = model;
            model.AddLog(new Log("INFO", "opened new dir"));
            
            
        }
    }
}
