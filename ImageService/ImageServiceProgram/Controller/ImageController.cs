using ImageServiceProgram.Commands;
using ImageServiceProgram.Infrastructure;
using ImageServiceProgram.Infrastructure.Enums;
using ImageServiceProgram.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal Modal;                      // The Modal Object
        private Dictionary<int, ICommand> Commands;

        public ImageController(IImageServiceModal modal)
        {
            this.Modal = modal;                    // Storing the Modal Of The System
            Commands = new Dictionary<int, ICommand>()
            {
                {0, new NewFileCommand(this.Modal) }
				// For Now will contain NEW_FILE_COMMAND
            };
        }
        public string ExecuteCommand(int commandID, string[] args, out bool result)
        {
            if (Commands.ContainsKey(commandID))
            {
                result = true;
                return this.Commands[commandID].Execute(args, out result);
            }
            else
            {
                result = false;                                 //not sure this makes sense. Is this really the functionality of the result?
                return "Command not found";
            }

        }
    }
}
