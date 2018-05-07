using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.communication
{
    public class Log
    {
        private string type;
        public string Type { get { return type; } set { type = value; } }

        private string message;
        public string Message { get {return message; } set { message = value; } }

        public Log(string type, string msg)
        {
            this.type = type;
            this.message = msg;
        }
    }
}
