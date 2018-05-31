using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GUI.TcpClient;

namespace GUI.Model
{
    public interface ILogModel
    {

        ObservableCollection<Log> LogMessages { get; set; }

	}
}
