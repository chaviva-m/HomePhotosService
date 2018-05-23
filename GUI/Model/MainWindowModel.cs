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

		private string backgroundColor;
		public string BackgroundColor
		{
			get { return backgroundColor; }
			set {
				backgroundColor = value;
				OnPropertyChanged("BackgroundColor");
			}
		}

		public MainWindowModel()
		{
			BackgroundColor = ChooseBackgroundColor();
		}

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
