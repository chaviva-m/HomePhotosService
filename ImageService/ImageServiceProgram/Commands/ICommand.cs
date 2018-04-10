using ImageServiceProgram.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// execute command 
        /// </summary>
        /// <param name="args the args for the command"></param>
        /// <param name="result of command"></param>
        /// <returns>return string indicating if command was successful</returns>
        string Execute(string[] args, out bool result);
    }
}
