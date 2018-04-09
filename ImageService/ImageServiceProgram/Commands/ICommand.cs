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
        //execute command
        string Execute(string[] args, out bool result);
    }
}
