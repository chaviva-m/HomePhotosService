using ImageServiceProgram.Modal;
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


using ImageServiceProgram.Modal;
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

namespace ImageServiceProgram.Controller.Handlers
{
    public class DirectoryHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                             // The Path of directory
        private bool result;
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        // Implement Here!

        public DirectoyHandler(string dirPath, IImageController controller, ILoggingService logger)
        {
            this.m_path = dirPath;
            this.m_controller = controller;
            this.m_logging = logger;
            this.m_dirWatcher = new FileSystemWatcher();
            m_dirWatcher.Created += new FileSystemEventHandler(OnCreated);
        }

        /// <summary>
        /// set fileSystemWatcher to watch specific directory.
        /// </summary>
        /// <param name="dirPath"> directory path to watch</param>
        public void StartHandleDirectory(string dirPath)
        {
            this.m_dirWatcher.Path = dirPath;

        }

        /// <summary>
        /// activate controller once picture is added to watched directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"> that contains the event data.</param>
        private void OnCreated(FileSystemEventArgs e)
        {
            // get the file's extension 
            string strFileExt = Path.GetExtension(e.FullPath);

            // filter file types 
            if (Regex.IsMatch(strFileExt, @"\.jpg)|\.png|\.gif|\.bmp", RegexOptions.IgnoreCase))
            {
                string[] args = { e.FullPath };
                string msg = this.m_controller.ExecuteCommand(0, args, out this.result);
                MessageTypeEnum mte;

                //find out if command succeeded or not in order to inform logger
                if (this.result == false)
                {
                    mte = MessageTypeEnum.FAIL;
                }
                else
                {
                    mte = MessageTypeEnum.INFO;
                }

                this.m_logging.Log(msg, mte);
            }

        }


    }

    
}
