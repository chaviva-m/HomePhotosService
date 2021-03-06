﻿using CommandInfrastructure.Commands;
using CommandInfrastructure.Commands.Enums;
using GUI.TcpClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        private string outputDirectory;
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
        private string sourceName;
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
        private string logName;
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
        private string thumbnailSize;
        public string ThumbnailSize
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
       
		/// <summary>
		/// constructor
		/// </summary>
        public SettingsModel()
        {
            ClientChannel clientChannel = ClientChannel.Instance;
            //add methods to client channel's event
            clientChannel.CommandReceived += GetAppConfig;
            clientChannel.CommandReceived += DeleteDir;
            //request app config settings
            string[] args = { "" };
            clientChannel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, args, ""));
        }

		/// <summary>
		/// if command is GetConfigCommand, sets all values of settings according to cmdArgs
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="cmdArgs">commmand args</param>
		private void GetAppConfig(object sender, CommandReceivedEventArgs cmdArgs)
        { 
            if (cmdArgs.CommandID == (int)CommandEnum.GetConfigCommand)
            {
				//set all properties to values in args from client channel
				OutputDirectory = cmdArgs.Args[0];
				SourceName = cmdArgs.Args[1];
				LogName = cmdArgs.Args[2];
				ThumbnailSize = cmdArgs.Args[3];
				for (int i = 4; i < cmdArgs.Args.Length; i++)
				{
					AddDir(cmdArgs.Args[i]);
				}
			}
        }

		/// <summary>
		/// if command is CloseDirectoryCommand, delets relevant directory from directories
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <param name="cmdArgs">commmand args</param>
		private void DeleteDir(object sender, CommandReceivedEventArgs cmdArgs)
        {
            if (cmdArgs.CommandID == (int)CommandEnum.CloseDirectoryCommand)
            {
                directories.Remove(cmdArgs.RequestDirPath);
            }
        }

		/// <summary>
		/// add directory to directories
		/// </summary>
		/// <param name="dirToAdd">the name of the directory to add</param>
        private void AddDir(string dirToAdd)
        {
            directories.Add(dirToAdd);
        }
    }
}
