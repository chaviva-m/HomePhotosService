//using ImageServiceProgram.Controller.Handlers;
//using ImageServiceProgram.Infrastructure;
//using ImageServiceProgram.Logging;
//using ImageServiceProgram.Modal;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ImageServiceProgram.Commands
//{
//    public class CloseDirectoryCommand : ICommand
//    {

//        //private FileSystemWatcher dirWatcher;

//        //private EventHandler directoryClose;

//        //public delegate string handlerCloser(out bool result);
//        //public event handlerCloser Log;

//        public CloseDirectoryCommand(/*FileSystemWatcher watcher*/ DirectoryHandler h)
//        {
//            //dirWatcher = watcher;
//            //directoryClose = DirClose;
//        }

//        public string Execute(string[] args, out bool result)
//        {
//            //de
//            dirWatcher.Dispose();       //is this the function we need? documention wasn`t clear
//            //directoryClose?.Invoke(this,)
//            // The String Will Return the New Path if result = true, and will return the error message
            
//        }
//    }
//}
