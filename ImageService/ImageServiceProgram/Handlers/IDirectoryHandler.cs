using ImageServiceProgram.Modal;
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

        //not clear when StartHandleDirectory should happen- according to Word doc:dirPath is given in constructor and
        //this happens automatically (which makes sense!!)
        //but according to the interface this functionality is delegated to a different function. Weird.
        //I implemented it by the word doc that made sense :/

        void StartHandleDirectory(string dirPath);             // The Function Recieves the directory to Handle


        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);     // The Event that will be activated upon new Command
    }
}
