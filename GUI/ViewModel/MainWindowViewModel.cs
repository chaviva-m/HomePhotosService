using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using GUI.Model;

namespace GUI.ViewModel
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private IMainWindowModel windowModel;

		public string VM_BackgroundColor { get { return windowModel.BackgroundColor; } }

		public MainWindowViewModel(IMainWindowModel model)
		{
			windowModel = model;

			windowModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				NotifyPropertyChanged("VM_" + e.PropertyName);
			};
		}

		protected void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
