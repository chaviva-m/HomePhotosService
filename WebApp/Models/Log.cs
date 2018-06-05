using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Log
    {
        //type of log
        private string type;
        public string Type { get { return type; } set { type = value; } }
        //message of log
        private string message;
        public string Message { get { return message; } set { message = value; } }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public Log(string type, string msg)
        {
            this.type = type;
            this.message = msg;
        }
    }
}