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

		//properties that correspond to properties in model
        public string VM_OutputDirectory { get { return settingsModel.OutputDirectory; } }
        public string VM_SourceName { get { return settingsModel.SourceName; } }
        public string VM_LogName { get { return settingsModel.LogName; } }
        public string VM_ThumbnailSize { get { return settingsModel.ThumbnailSize; } }
        public string VM_DirToRemove {
            get { return settingsModel.DirToRemove; } set { settingsModel.DirToRemove = value; } }
        public ObservableCollection<string> VM_Directories { get { return settingsModel.Directories; } }

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="model">ISettingsModel</param>
        public SettingsViewModel(ISettingsModel model)
        {
			//set RemoveCommand for remove butto with method that will check 
			//if it can be pressed and method that will be called if it is pressed 
            RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
			//set model
            settingsModel = model;
            settingsModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

		/// <summary>
		/// when remove button is pressed, send command to server to remove chosen directory
		/// </summary>
		/// <param name="sender">the sender object</param>
		private void OnRemove(object sender)
        {
            ClientChannel channel = ClientChannel.Instance;
            string[] args = { "" };
            channel.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.CloseDirectoryCommand, args, VM_DirToRemove));
        }

		/// <summary>
		/// check if remove button can be pressed.
		/// </summary>
		/// <param name="sender">the sender object</param>
		/// <returns>true if a directory was selected, otherwise false</returns>
        private bool CanRemove(object sender)
        {
            if (string.IsNullOrEmpty(VM_DirToRemove))
             {
                 return false;
             }
             return true;
        }

		/// <summary>
		/// notify that property changed.
		/// </summary>
		/// <param name="name">name of property</param>
		protected void NotifyPropertyChanged(string name)
        {
			//enable remove command (for remove button) invoker to check if the command can execute.
			var command = this.RemoveCommand as DelegateCommand<object>;
            command.RaiseCanExecuteChanged();
			//notify property change
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}