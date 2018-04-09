using ImageServiceProgram.Commands;
using ImageServiceProgram.Controller;
using ImageServiceProgram.Infrastructure.Enums;
using ImageServiceProgram.Logging;
using ImageServiceProgram.ImageModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceProgram.Handlers;

namespace ImageServiceProgram.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController Controller;
        private ILoggingService Logger;
        #endregion

        #region Properties
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;          // The event that notifies about a new Command being recieved
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller"> the controller</param>
        /// <param name="logger"> the logger</param>
        public ImageServer(IImageController controller, ILoggingService logger)
        {
            this.Controller = controller;
            this.Logger = logger;
        }

        /// <summary>
        /// create a handler
        /// </summary>
        /// <param name="directory">the directory the handler will operate on</param>
        public void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoryHandler(Controller, Logger);
            CommandReceived += handler.OnCommandReceived;
            handler.DirectoryClose += onClose;
            handler.StartHandleDirectory(directory);
        }

        /// <summary>
        /// send a command to all handlers
        /// </summary>
        /// <param name="commandArgs">command details</param>
        public void SendCommand(CommandReceivedEventArgs commandArgs)               
        {
            CommandReceived?.Invoke(this, commandArgs);                            
        }

        /// <summary>
        /// when server closes
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="args">arguments relevant to server shut down</param>
        public void onClose(object sender, DirectoryCloseEventArgs args)
        {
            DirectoryHandler handler = (DirectoryHandler)sender;
            handler.DirectoryClose -= onClose;
            CommandReceived -= handler.OnCommandReceived;
        }      
    }
}