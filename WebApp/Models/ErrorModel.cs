using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
	public class ErrorModel
	{
		private string errorMsg;
		public string ErrorMsg { get { return errorMsg; } set { errorMsg = value; } }

		public ErrorModel(string msg)
		{
			ErrorMsg = msg;
		}

		public ErrorModel()
		{
			ErrorMsg = "An error has been encountered\n";
		}
	}
}