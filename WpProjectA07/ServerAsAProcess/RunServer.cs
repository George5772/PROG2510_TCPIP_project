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
		//contains stream writers for clients
		public static List<StreamWriter> ClientWriters = new List<StreamWriter>();

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

			//receiver send stop code

			tr.Join();
		}

    }//end of RunServer
}