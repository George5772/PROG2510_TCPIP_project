using System.Net.Sockets;

namespace ServerAsAProcess
{
    /*
	* FILE : WorkerSender.cs
	* PROJECT : PROG2510 - A07
	* PROGRAMMER : George Shapka
	* FIRST VERSION : 11/18/2025 12:04:15 PM
	*/

    /*
	* NAME : WorkerSender
	* PURPOSE : holds methods for the server thread responsible for sending messages
	*/
    internal class WorkerSender
    {
		/// <summary>
		/// takes the message from the client and sends it to all other clients
		/// </summary>
		public static void SendMessage(TcpClient client)
		{
			//compare client to client list

			//send message to all clients that do not match

			//remove client from list

			//disconnect from client
			return;
		}
    }//end of WorkerSender
}