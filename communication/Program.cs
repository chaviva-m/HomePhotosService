
using Communication.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] args1 = { "yes", "no" };
            CommandReceivedEventArgs cmdArgs = new CommandReceivedEventArgs(1, args1, "path");
            cmdArgs.practice(cmdArgs);
        }
    }
}
