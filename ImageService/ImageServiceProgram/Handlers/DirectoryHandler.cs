using ImageServiceProgram.ImageModal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageServiceProgram.Infrastructure;
using ImageServiceProgram.Infrastructure.Enums;
using ImageServiceProgram.Logging;
using ImageServiceProgram.Logging.Modal;
using System.Text.RegularExpressions;
using ImageServiceProgram.Commands;
using ImageServiceProgram.Controller;

namespace ImageServiceProgram.Handlers
{
    public class DirectoryHandler : IDirectoryHandler
    {
        #region Members
        private IImageController controller;              // The Image Processing Controller
        private ILoggingService logging;
        private FileSystemWatcher dirWatcher;             // The Watcher of the Dir
        private string path;                             // The Path of directory
        private Dictionary<int, ICommand> Commands;
        private CommandReceivedEventArgs eventArgs;
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed
        


        public DirectoryHandler(IImageController icontroller, ILoggingService logger)
        {
            path = "";
            controller = icontroller;
            logging = logger;
            dirWatcher = new FileSystemWatcher();
            dirWatcher.Created += new FileSystemEventHandler(OnCreated);

        }

        /// <summary>
        /// set fileSystemWatcher to watch specific directory.
        /// </summary>
        /// <param name="dirPath"> directory path to watch</param>
        public void StartHandleDirectory(string dirPath)
        {
            path = dirPath;
            dirWatcher.Path = dirPath;

        }

        /// <summary>
        /// activate controller once picture is added to watched directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"> that contains the event data.</param>
        protected void OnCreated(object sender, FileSystemEventArgs e)
        {
            // get the file's extension 
            string strFileExt = Path.GetExtension(e.FullPath);

            // filter file types 
            if (Regex.IsMatch(strFileExt, @"\.jpg)|\.png|\.gif|\.bmp", RegexOptions.IgnoreCase))
            {
                string[] args = { e.FullPath };
                bool result;
                string msg = controller.ExecuteCommand(0, args, out result);
                MessageTypeEnum mte;

                //find out if command succeeded or not in order to inform logger
                if (result == false)
                {
                    mte = MessageTypeEnum.FAIL;
                }
                else
                {
                    mte = MessageTypeEnum.INFO;
                }

                logging.Log(msg, mte);
            }

        }

        public void OnCommandReceived(object sender, EventArgs e)
        {
            bool result;
            string msg;
            if (e.GetType() == typeof(DirectoryCloseEventArgs))
            {
                //make sure command was meant for the handler for the specific path
                DirectoryCloseEventArgs closeEvent = (DirectoryCloseEventArgs)e;
                if (closeEvent.DirectoryPath != path)
                {
                    return;
                }

                //DirectoryCloseEventArgs closeEvent = (DirectoryCloseEventArgs)e;
                msg = closeHandler(out result);

            }
            //a command was passed that was NOT to close directory
            else
            {
                CommandReceivedEventArgs commandEvent = (CommandReceivedEventArgs)e;
                //make sure command was meant for the handler for the specific path
                if (commandEvent.RequestDirPath != path)
                {
                    return;
                }


                msg = Commands[commandEvent.CommandID].Execute(commandEvent.Args, out result);
            }
                MessageTypeEnum mte;

                //find out if command succeeded or not in order to inform logger
                if (result == false)
                {
                    mte = MessageTypeEnum.FAIL;
                }
                else
                {
                    mte = MessageTypeEnum.INFO;
                }

                //inform logger
                logging.Log(msg, mte);

        }



        public string closeHandler(out bool result) //this should get DirectoryCloseEventArgs -------
        {
           
            result = true;

            //disable fileSystemWatcher
            dirWatcher.Created -= OnCreated;
            dirWatcher.Dispose();

            string msg = "stopped monitoring path" + path;

            //evoke event
            DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(path, msg));

            return msg;
        }


    }

    
}
