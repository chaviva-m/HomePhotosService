using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GUI.communication;

namespace GUI.Model
{
    interface ILogModel
    {

        ObservableCollection<Log> LogMessages { get; set; }

        void AddLog(Log l);
    }
}
