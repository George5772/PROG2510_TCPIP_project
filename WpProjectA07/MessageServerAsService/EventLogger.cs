using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageServerAsService
{
    public static class EventLogger
    {
        /// <summary>
        /// this method use to log messaege to event log for debug purpose
        /// </summary>
        /// <param name="message">message need to send</param>
        public static void Log(string message)
        {
            EventLog serviceEventLog = new EventLog();
            if (!EventLog.SourceExists("MessageServerService"))
            {
                EventLog.CreateEventSource("MessageServerService", "MessageServerEventLog");
            }
            serviceEventLog.Source = "MessageServerService";
            serviceEventLog.Log = "MessageServerEventLog";
            serviceEventLog.WriteEntry(message);
            return;
        }
    }
}
