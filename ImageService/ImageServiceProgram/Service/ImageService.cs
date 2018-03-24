﻿using ImageServiceProgram.Logging;
using ImageServiceProgram.Logging.Modal;
using ImageServiceProgram.Modal;
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
        private int eventId = 1;
        private ImageServer imageServer;          // The Image Server
        private IImageServiceModal imageModal;
        private IImageController controller;
        private ILoggingService logger;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public ImageService()
        {
            InitializeComponent();
            string eventSourceName = "MySource";
            string logName = "MyNewLog";
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

        // Here You will Use the App Config!
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.   
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            eventLog.WriteEntry("In OnStart");
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //create logger and add OnMessage to logging event
            logger = new LoggingService();
            logger.MessageRecieved += onMessage;
            //create ImageModal
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            int thumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            imageModal = new ImageServiceModal(outputDir, thumbnailSize);
            //create controller
            controller = new ImageController(imageModal);
            //create server
            imageServer = new ImageServer(controller, logger); 

        }

        protected override void OnStop()
        {
            // Update the service state to stop pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog.WriteEntry("In onStop.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("In OnContinue.");
        }

        protected void onMessage(object sender, MessageReceivedEventArgs args)
        {
            switch(args.Status)
            {
                case MessageTypeEnum.INFO:
                        eventLog.WriteEntry("Info: "); //take this out?
                        break;
                case MessageTypeEnum.WARNING:
                        eventLog.WriteEntry("Warning: ");
                        break;
                case MessageTypeEnum.FAIL:
                        eventLog.WriteEntry("Fail: ");
                        break;
            }
            eventLog.WriteEntry(args.Message);
        }

        private void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}