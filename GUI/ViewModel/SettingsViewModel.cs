using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GUI.Model;
using Microsoft.Practices.Prism.Commands;


namespace GUI.ViewModel
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RemoveCommand { get; private set; }

        public SettingsViewModel(ISettingsModel settingsModel)
        {
            this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
        }

        private void OnRemove(object obj)
        {
            //send command through model
        }

        private bool CanRemove(object obj)
        {
            return true;
        }

        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
