using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandInfrastructure.Commands
{
    public class CommandReceivedEventArgs
    {
        public int CommandID { get; set; }      // The Command ID
        public string[] Args { get; set; }
        public string RequestDirPath { get; set; }  // The Request Directory

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id">command id</param>
        /// <param name="args">command arguments</param>
        /// <param name="path">directory path</param>
        public CommandReceivedEventArgs(int id, string[] args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }
    }
}

