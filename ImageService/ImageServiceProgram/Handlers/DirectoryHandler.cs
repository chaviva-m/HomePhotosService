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
        private CommandReceivedEventArgs eventArgs;
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed
        

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="icontroller">controller</param>
        /// <param name="logger">logger</param>
        public DirectoryHandler(IImageController icontroller, ILoggingService logger)
        {
            path = "";
            controller = icontroller;
            logging = logger;
        }

        /// <summary>
        /// set fileSystemWatcher to watch specific directory.
        /// </summary>
        /// <param name="dirPath"> directory path to watch</param>
        public void StartHandleDirectory(string dirPath)
        {
            path = dirPath;
            dirWatcher = new FileSystemWatcher();
            dirWatcher.Path = dirPath;
            dirWatcher.Created += new FileSystemEventHandler(OnCreated);

            try
            {
                dirWatcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                logging.Log("fileSystemWatcher initialization failed: " + e.Message, MessageTypeEnum.FAIL);
            }

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
            if (Regex.IsMatch(strFileExt, @"\.jpg|\.png|\.gif|\.bmp", RegexOptions.IgnoreCase))
            {
                string[] args = { e.FullPath };
                CommandReceivedEventArgs commandReceived = new CommandReceivedEventArgs((int)CommandEnum.NewFileCommand, args, path);
                OnCommandReceived(this, commandReceived);
            }

        }

        /// <summary>
        /// command execution
        /// </summary>
        /// <param name="sender">object that sent the command</param>
        /// <param name="e">command received event args</param>
        public void OnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            bool result;
            string msg;
            
            //make sure command was meant for the handler for the specific path
            if (e.RequestDirPath != path)
            {
                return;
            }

            if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                //close the handler
                msg = closeHandler(out result);

            }
            //a command was passed that was not to close directory
            else
            {                
                msg = controller.ExecuteCommand(e.CommandID,e.Args, out result);
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

        /// <summary>
        /// handler close
        /// </summary>
        /// <param name="result">true-if close succeeded, false otherwise</param>
        /// <returns>success/failure information for logger</returns>
        private string closeHandler(out bool result)
        {
            string msg;
            result = true;
            try
            {
                //disable fileSystemWatcher 
                dirWatcher.EnableRaisingEvents = false;
                msg = "stopped monitoring path " + path;
                result = true;
            } catch (Exception e)
            {
                result = false;
                msg = "could not quit monitoring path " + path;
            }            
            //evoke event
            DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(path, msg));
            return msg;
        }
    }    
}
