using System.Net.Sockets;

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
		public static List<TcpClient> Clients = new List<TcpClient>();

		/// <summary>
		/// holds main server loop
		/// </summary>
		public static void ServerStart()
		{
			ThreadStart trStart = new ThreadStart(WorkerReceiver.ReceiveMessage);
			Thread tr = new Thread(trStart);
			tr.Start();

			//receiver send stop code

			tr.Join();
		}
    }//end of RunMainProgram
}