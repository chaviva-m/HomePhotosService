using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GUI.Model
{
	interface IMainWindowModel : INotifyPropertyChanged
	{
		string BackgroundColor { get; set; }
	}
}
