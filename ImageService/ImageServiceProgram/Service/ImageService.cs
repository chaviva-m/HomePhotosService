using ImageServiceProgram.Logging;
using ImageServiceProgram.Logging.Modal;
using ImageServiceProgram.ImageModal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageServiceProgram.Controller;
using ImageServiceProgram.Server;
using ImageServiceProgram.Infrastructure.Enums;

namespace ImageServiceProgram.Service
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    public partial class ImageService : ServiceBase
    {
        private ImageServer imageServer;      
        private IImageServiceModal imageModal;
        private IImageController controller;
        private ILoggingService logger;
        private string[] directories;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        /// <summary>
        /// ImageService constructor
        /// sets event log.
        /// </summary>
        public ImageService()
        {
            InitializeComponent();
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];
            eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog.Source = eventSourceName;
            eventLog.Log = logName;
        }      

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        /// <summary>
        /// starts service, updates to running.
        /// Creates logger, imageModal, server and handlers.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.   
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            eventLog.WriteEntry("In OnStart");
            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //create logger and add OnMessage to logging event
            logger = new LoggingService();
            logger.MessageRecieved += OnMessage;          
            //create ImageModal
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            int thumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            imageModal = new ImageServiceModal(outputDir, thumbnailSize);
            //create controller
            controller = new ImageController(imageModal);
            //create server and handlers for each directory in app configuration
            imageServer = new ImageServer(controller, logger);
            directories = ConfigurationManager.AppSettings["Handler"].Split(';');
            foreach (string directory in directories)
            {
                imageServer.CreateHandler(directory);
            }
        }

        /// <summary>
        /// Server termination. Updates the handler that service is closed.
        /// </summary>
        protected override void OnStop()
        {
            // Update the service state to stop pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog.WriteEntry("In OnStop.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //close directory handlers
            string[] args = { "" };
            foreach (string directory in directories)
            {
                imageServer.SendCommand(new CommandReceivedEventArgs((int)CommandEnum.CloseCommand, args, directory));
            }           
        }


        protected override void OnContinue()
        {
            eventLog.WriteEntry("In OnContinue.");
        }

        /// <summary>
        /// writes entry to logger.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args: has message for logger and type of message"></param>
        protected void OnMessage(object sender, MessageReceivedEventArgs args)
        {
            switch(args.Status)
            {
                case MessageTypeEnum.INFO:
                    eventLog.WriteEntry(args.Message, EventLogEntryType.Information);
                    break;
                case MessageTypeEnum.WARNING:
                    eventLog.WriteEntry(args.Message, EventLogEntryType.Warning);
                    break;
                case MessageTypeEnum.FAIL:
                    eventLog.WriteEntry(args.Message, EventLogEntryType.FailureAudit);
                    break;
            }
        }
    }
}
