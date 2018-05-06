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
            handler.DirectoryClose += onHandlerClose;
            handler.StartHandleDirectory(directory);
        }

        /// <summary>
        /// send a command to all handlers
        /// </summary>
        /// <param name="commandArgs">command details</param>
        public void SendCommand(CommandReceivedEventArgs commandArgs)   //change to SendHandlerCommand, for directory oriented commands              
        {
            CommandReceived?.Invoke(this, commandArgs);
        }

        /// <summary>
        /// when server closes
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="args">arguments relevant to server shut down</param>
        public void onHandlerClose(object sender, DirectoryCloseEventArgs args)
        {
            DirectoryHandler handler = (DirectoryHandler)sender;
            handler.DirectoryClose -= onHandlerClose;
            CommandReceived -= handler.OnCommandReceived;
        }
    }
}

    //PASS CommandReceivedEventArgs to new project and make serialiazable
    //open thread always listening to clients(connect/listen)
    //if client connects-add to client list and open new thread to carry out client request (CommandReceivedEventArgs)
    //for all commands received by server: check if has path- if yes:  invoke sendHandlerCommand (pass to handlers) , if not - to controller via new method sendControllerCommand
    //make SendCLientLog(object sender, messageReceivedEventArgs mr) translates it into CommandReceivedEventArgs and send to generic SendClientCommand(int clientid,CommandReceivedEventArgs cr) .
    //service makes dictionary with:
    //new file commandCommand(ImageModal)

    //logCommand (hold server) -gets  list of log history from the singelton which holds a list of
    //MessageReceivedEventArgs and iterates over list sending each one to sendClientLog by invoking 
    //SendCLientLog from server (extracts client id from the end of the list)
    //
    //SendCLientsLog(object sender, messageReceivedEventArgs mr) subscribes to singelton, iterates over all clients and sends mr
    //SendCLientLog(object sender, messageReceivedEventArgs mr, int clientid)

    //when server received command it will add client id as last elemnt in args[] by using:
    //Array.Resize(ref array, newsize);
    //array[newsize - 1] = "newvalue"
    //app config and logComand will remember this and extract client from the end of args and send data to
    //relevant client only

    //(singelton appConfigSingelton which holds all data from appConfig in shape of CommandReceivedEventArgs)
    //config command gets data from singleton and hold server -invokes sendClienCommand in server
    //(get the id from args [] - it will be the only string in the array