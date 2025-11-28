/*
* FILE : EventLogger.cs
* PROJECT : PROG2510 - A07
* PROGRAMMER : George Shapka, Le Hai Quy Bui
* FIRST VERSION : 11/14/2025
*/
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
            if (!EventLog.SourceExists("MyEventSource"))
            {
                EventLog.CreateEventSource("MyEventSource", "MyEventLog");
            }
            serviceEventLog.Source = "MyEventSource";
            serviceEventLog.Log = "MyEventLog";
            serviceEventLog.WriteEntry(message);
            return;
        }
    }
}
