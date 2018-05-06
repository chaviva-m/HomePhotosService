using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Commands.Enums
{
    public enum CommandEnum : int
    {
        NewFileCommand,     //add new file to output directory
        GetConfigCommand,   //get settings in app config
        LogHistoryCommand,  //get all logs from start of service
        LogUpdateCommand,   //get most recent log 
        CloseCommand        //close directory
    }
}