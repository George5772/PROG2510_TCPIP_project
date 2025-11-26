using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MessageServerAsService
{
    public partial class MessageServerService: ServiceBase
    {
        RunServer runServer;
        public MessageServerService()
        {

            InitializeComponent();
            runServer = new RunServer();
            CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            EventLogger.Log(LoggerActions.ServiceStarted + "Service started");
            Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.ServiceStarted, "Service started");
            runServer.ServerStart();
        }
 
        protected override void OnStop()
        {
            EventLogger.Log(LoggerActions.ServideStopped + "Service stopped");
            Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.ServideStopped, "Service stopped");
            runServer.ServerContinue(); 
            runServer.ServerStop();
        }
        protected override void OnContinue()
        {
            Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.ServiceResumed, "Service resumed.");
            EventLogger.Log(LoggerActions.ServiceResumed + "Service Resume");
            runServer.ServerContinue();
        }
        protected override void OnPause()
        {
            Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.ServicePaused, "Service paused");
            EventLogger.Log(LoggerActions.ServicePaused + "Service paused");
            runServer.ServerPause();
           
        }
    }
}
