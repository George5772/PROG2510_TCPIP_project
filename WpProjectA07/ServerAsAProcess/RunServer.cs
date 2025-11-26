using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace ServerAsAProcess
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
		public static List<TcpClient> Clients = new List<TcpClient>();
		public static string LogFilePath = "C:\\logs\tcpProject.log";


        public static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        public static Int32 port = 13000;

        //contains threads for sender threads from the clients
        public static List<Thread> Threads = new List<Thread>();

		/// <summary>
		/// starts the server
		/// </summary>
		public static void ServerStart()
		{
			//start receiver thread
			ThreadStart trStart = new ThreadStart(WorkerReceiver.ReceiveMessage);
			Thread tr = new Thread(trStart);
			tr.Start();
			Console.ReadKey();
			sendStop();
			WorkerReceiver.stop = true;


            tr.Join();
		}

		public static void sendStop()
		{
            TcpClient senderClient = new TcpClient(localAddr.ToString(), port);
            StreamWriter senderWriter = new StreamWriter(senderClient.GetStream(), System.Text.Encoding.ASCII);
            senderWriter.AutoFlush = true;
            senderWriter.WriteLine("Stop");
			senderClient.Close();
        }

    }//end of RunServer
}