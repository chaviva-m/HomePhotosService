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
using System.Windows;

namespace GUI.Model
{
    public class SettingsModel : ISettingsModel
    {

        //notify change
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //output directory
        private string outputDirectory;// = "outputDir"; //change this
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
        private string sourceName;// = "src"; //change this
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
        private string logName;// = "log"; //change this
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
        private int thumbnailSize;// = 120; //change this
        public int ThumbnailSize
        {
            get { return thumbnailSize; }
            set
            {
                thumbnailSize = value;
                OnPropertyChanged("ThumbnailSize");
            }
        }

        //directory list
        private string dirToRemove;
        public string DirToRemove
        {
            get { return dirToRemove; }
            set
            {
                dirToRemove = value;
                OnPropertyChanged("DirToRemove");
            }
        }
        private ObservableCollection<string>  directories = new ObservableCollection<string>();
        public ObservableCollection<string> Directories {
            get { return directories; }
            set { directories = value; }
        }
       
        public SettingsModel()
        {
            ClientChannel clientChannel = ClientChannel.Instance;
            //add methods to client channel's event
            clientChannel.CommandReceived += GetAppConfig;
            clientChannel.CommandReceived += DeleteDir;
            //request app config settings

            Debug.WriteLine("sending command to get app config args");

            string[] args = { "" };
            clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, args, ""));
        }        
        
        private void GetAppConfig(object sender, CommandReceivedEventArgs cmdArgs)
        { 
            if (cmdArgs.CommandID != (int)CommandEnum.GetConfigCommand)
            {
                Debug.WriteLine("in getAppConfig: not relevant command");
                return;
            }

            //set all properties to values in args from client channel
            OutputDirectory = cmdArgs.Args[0];
            SourceName = cmdArgs.Args[1];
            LogName = cmdArgs.Args[2];
            ThumbnailSize = Int32.Parse(cmdArgs.Args[3]);
            for (int i = 4; i < cmdArgs.Args.Length; i++)
            {
                AddDir(cmdArgs.Args[i]);
            }
            Debug.WriteLine("got config from server");
        }

        private void DeleteDir(object sender, CommandReceivedEventArgs cmdArgs)
        {
            if (cmdArgs.CommandID == (int)CommandEnum.CloseCommand)
            {
                directories.Remove(cmdArgs.RequestDirPath);
            }
        }

        //erase
        public void DeleteDir(string dirToRemove)       //make this private
        {
            directories.Remove(dirToRemove);
        }

        public void AddDir(string dirToAdd)       //make this private
        {
            directories.Add(dirToAdd);
        }
    }
}
