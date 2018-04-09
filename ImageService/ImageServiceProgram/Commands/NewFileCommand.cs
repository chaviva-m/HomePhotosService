using ImageServiceProgram.Infrastructure;
using ImageServiceProgram.ImageModal;
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
            // The String Will Return the message with new Path if result = true, else will return the error message        
            return Modal.AddFile(args[0], out result);
        }
    }
}
