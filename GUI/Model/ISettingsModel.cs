using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
    interface ISettingsModel : INotifyPropertyChanged
    {
        string OutputDirectory { get; set; }
        string SourceName { get; set; }
        string LogName { get; set; }
        int ThumbnailSize { get; set; }
        ObservableCollection<string> Directories { get; set; }
        string DirToRemove { get; set; }
        void DeleteDir(string dirToRemove);                         //take this out
        void AddDir(string dirToAdd);                               //take this out

    }
}
