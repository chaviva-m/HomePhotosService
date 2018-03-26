using ImageServiceProgram.Infrastructure;
using ImageServiceProgram.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal Modal;

        public NewFileCommand(IImageServiceModal modal)
        {
            Modal = modal;            // Storing the Modal
        }

        public string Execute(string[] args, out bool result)
        {
            // The String Will Return the New Path if result = true, and will return the error message
        }
    }
}
