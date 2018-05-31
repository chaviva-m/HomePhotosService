using GUI.TcpClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
	class MainWindowModel : IMainWindowModel
	{
		//notify change
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		//background color of main window
		private string backgroundColor;
		public string BackgroundColor
		{
			get { return backgroundColor; }
			set {
				backgroundColor = value;
				OnPropertyChanged("BackgroundColor");
			}
		}

		/// <summary>
		/// constructor
		/// </summary>
		public MainWindowModel()
		{
			BackgroundColor = ChooseBackgroundColor();
		}

		/// <summary>
		/// chooses background color according to connection state of client.
		/// </summary>
		/// <returns>white if client is connected, otherwise gray</returns>
		private string ChooseBackgroundColor()
		{
			ClientChannel client = ClientChannel.Instance;
			if (client.IsConnected)
			{
				return "white";
			} else
			{
				return "gray";
			}
		}
	}
}
