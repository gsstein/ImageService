﻿using System;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Modal;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Configuration;
using ImageService.Infrastructure;
using System.IO;


namespace ImageService
{
    public partial class ImageService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private ImageServer ImageServer;          // The Image Server
        private ILoggingService Logging;
        private ServiceStatus Status;

        public ImageService(string[] args)
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


        /// <summary>
        /// Creates auxiliary LoggingService and ImageServer
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            Status = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(this.ServiceHandle, ref Status);
            LogStatus();

            Logging = new LoggingService();
            Logging.MessageReceived += OnMessage;
            ImageServer = new ImageServer(Logging);

            // Update the service state to Running.  
            Status.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref Status);
            LogStatus();

            // Start Server
            ImageServer.Start();
        }

        private void LogStatus()
        {
            eventLog.WriteEntry(Status.dwCurrentState.ToString());
        }

        /// <summary>
        /// Invoked on MessageReceived event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            eventLog.WriteEntry(e.Message);
        }

        protected override void OnStop()
        {
            ImageServer.Stop();
            Status.dwCurrentState = ServiceState.SERVICE_STOPPED;
            LogStatus();
        }
    }

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
}
