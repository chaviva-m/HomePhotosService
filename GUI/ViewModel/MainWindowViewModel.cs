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

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="model">IMainWindowModel</param>
		public MainWindowViewModel(IMainWindowModel model)
		{
			windowModel = model;
			windowModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				NotifyPropertyChanged("VM_" + e.PropertyName);
			};
		}

		/// <summary>
		/// notify that property changed.
		/// </summary>
		/// <param name="name">name of property</param>
		protected void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
