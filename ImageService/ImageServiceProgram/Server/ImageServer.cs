using ImageServiceProgram.Controller;
using ImageServiceProgram.Controller.Handlers;
using ImageServiceProgram.Infrastructure.Enums;
using ImageServiceProgram.Logging;
using ImageServiceProgram.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController controller;
        private ILoggingService logger;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public ImageServer(IImageController controller, ILoggingService logger)
        {
            this.controller = controller;
            this.logger = logger;
        }

       
    }
}
