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

		/// <summary>
		/// construct error model with error message
		/// </summary>
		/// <param name="msg">the error message</param>
		public ErrorModel(string msg)
		{
			ErrorMsg = msg;
		}

		/// <summary>
		/// default constructor
		/// </summary>
		public ErrorModel()
		{
			ErrorMsg = "An error has been encountered\n";
		}
	}
}