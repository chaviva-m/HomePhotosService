﻿using ImageServiceProgram.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceProgram.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        void StartHandleDirectory(string dirPath);             // The Function Recieves the directory to Handle


        void OnCommandReceived(object sender, EventArgs e);     // The Event that will be activated upon new Command
    }
}
