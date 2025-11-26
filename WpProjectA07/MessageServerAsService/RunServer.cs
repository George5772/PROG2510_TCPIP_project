using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageServerAsService
{
    /*
	* FILE : RunServer.cs
	* PROJECT : PROG2510 - A07
	* PROGRAMMER : George Shapka
	* FIRST VERSION : 11/18/2025 12:02:39 PM
	*/

    /*
	* NAME : RunServer
	* PURPOSE : Runs the program
	*/
    internal class RunServer
    {
        //contains tcpClients for receiver clients
        public static ManualResetEvent OkayToContinue = new ManualResetEvent(true);
        public static List<TcpClient> Clients = new List<TcpClient>();
        public static string LogFilePath = "C:\\logs\tcpProject.log";


        public static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        public static Int32 port = 13000;

        //contains threads for sender threads from the clients
        public static List<Thread> Threads = new List<Thread>();

        ThreadStart trStart;
        Thread tr;

        /// <summary>
        /// starts the server
        /// </summary>
        public void ServerStart()
        {
            //start receiver thread
            trStart = new ThreadStart(WorkerReceiver.ReceiveMessage);
            tr = new Thread(trStart);
            tr.Start();

        }

        public void ServerStop()
        {
            sendStop();
            WorkerReceiver.stop = true;
            tr.Join();
        }
        public void ServerPause()
        {
            OkayToContinue.Reset();
        }
        public void ServerContinue()
        {
            OkayToContinue.Set();
        }
        public static void sendStop()
        {
            TcpClient senderClient = new TcpClient(localAddr.ToString(), port);
            StreamWriter senderWriter = new StreamWriter(senderClient.GetStream(), System.Text.Encoding.ASCII);
            senderWriter.AutoFlush = true;
            senderWriter.WriteLine("Stop");
            senderClient.Close();
        }

    }
}
