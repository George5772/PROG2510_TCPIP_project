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
        }

        protected override void OnStart(string[] args)
        {
            runServer.ServerStart();
        }
 
        protected override void OnStop()
        {
            RunServer.sendStop();
        }
    }
}
