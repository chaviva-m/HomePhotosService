using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
    class SettingsModel : ISettingsModel
    {
        //need to get values from client

        //notify change
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //output directory
        private string outputDirectory = "outputDir"; //change this
        public string OutputDirectory
        {
            get { return outputDirectory; }
            set
            {
                outputDirectory = value;
                OnPropertyChanged("OutputDirectory");
            }
        }

        //source name
        private string sourceName = "src"; //change this
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                sourceName = value;
                OnPropertyChanged("SourceName");
            }
        }

        //log name
        private string logName = "log"; //change this
        public string LogName
        {
            get { return logName; }
            set
            {
                logName = value;
                OnPropertyChanged("LogName");
            }
        }

        //thumbnail size
        private int thumbnailSize = 120; //change this
        public int ThumbnailSize
        {
            get { return thumbnailSize; }
            set
            {
                thumbnailSize = value;
                OnPropertyChanged("ThumbnailSize");
            }
        }
    }
}
