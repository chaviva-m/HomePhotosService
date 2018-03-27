﻿using ImageServiceProgram.Commands;
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
        private IImageController Controller;
        private ILoggingService Logger;
        #endregion

        #region Properties
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;          // The event that notifies about a new Command being recieved
        #endregion

        public ImageServer(IImageController controller, ILoggingService logger)
        {
            this.Controller = controller;
            this.Logger = logger;
        }

        public void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoryHandler(Controller, Logger, directory);
            CommandReceived += handler.OnCommandReceived;
            handler.DirectoryClose += onClose;
        }

        public void SendCommand(CommandReceivedEventArgs commandArgs)               
        {
            CommandReceived?.Invoke(this, commandArgs);                            
        }

        public void onClose(object sender, DirectoryCloseEventArgs args)
        {
            DirectoryHandler handler = (DirectoryHandler)sender;
            handler.DirectoryClose -= onClose;
            CommandReceived -= handler.OnCommandRecieved;
        }      
    }
}