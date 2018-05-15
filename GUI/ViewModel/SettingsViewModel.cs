using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CommandInfrastructure.Commands;
using CommandInfrastructure.Commands.Enums;
using GUI.Model;
using GUI.TcpClient;
using Microsoft.Practices.Prism.Commands;


namespace GUI.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RemoveCommand { get; private set; }
        private ISettingsModel settingsModel;
        public ISettingsModel SettingsModel { get; set; }

        public string VM_OutputDirectory { get { return settingsModel.OutputDirectory; } }
        public string VM_SourceName { get { return settingsModel.SourceName; } }
        public string VM_LogName { get { return settingsModel.LogName; } }
        public int VM_ThumbnailSize { get { return settingsModel.ThumbnailSize; } }
        public string VM_DirToRemove {
            get { return settingsModel.DirToRemove; } set { settingsModel.DirToRemove = value; } }
        public ObservableCollection<string> VM_Directories { get { return settingsModel.Directories; } }

        public SettingsViewModel(ISettingsModel model)
        {
            RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
            settingsModel = model;
            settingsModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
            //settingsModel.AddDir("dir one");  //take this out
            //settingsModel.AddDir("dir two");  //take this out
            //settingsModel.AddDir("dir three");  //take this out
        }

        private void OnRemove(object sender)
        {
            ClientChannel channel = ClientChannel.Instance;
            string[] args = { "" };
            channel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.CloseDirectoryCommand, args, VM_DirToRemove));
        }

        private bool CanRemove(object sender)
        {
            if (string.IsNullOrEmpty(VM_DirToRemove))
             {
                 return false;
             }
             return true;
        }

        protected void NotifyPropertyChanged(string name)
        {
            var command = this.RemoveCommand as DelegateCommand<object>;
            command.RaiseCanExecuteChanged();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}