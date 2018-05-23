using CommandInfrastructure.Commands;
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

        /// <summary>
        /// newFileCommand constructor
        /// </summary>
        /// <param name="modal the image service modal"></param>
        public NewFileCommand(IImageServiceModal modal)
        {
            Modal = modal;            // Storing the Modal
        }

		/// <summary>
		/// execute newFileCommand
		/// </summary>
		/// <param name="args"> args[0] holds path of file to add</param>
		/// <param name="result">result of command</param>
		/// <returns>return string indicating if command was successful</returns>
		public string Execute(string[] args, out bool result)
        {
            // The String Will Return the message with new Path if result = true, else will return the error message     
            return Modal.AddFile(args[0], out result);
        }
    }
}
