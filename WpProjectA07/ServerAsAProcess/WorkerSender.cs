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
		public static void SendMessage(object? client)
		{
            TcpClient? tcpClient = (TcpClient?)client;
            while (tcpClient != null)
            {
                Mutex? mut;
                byte[] message = System.Text.Encoding.ASCII.GetBytes(WorkerReceiver.GetStringFromNetworkStream(tcpClient));
                if (!Mutex.TryOpenExisting("A07Mutex", out mut))
                {
                    mut = new Mutex(true, "A07Mutex");
                    mut.ReleaseMutex();
                }
                mut.WaitOne();
                //compare client to client list and send message to other clients
                foreach (TcpClient c in RunServer.Clients)
                {
                    if (!c.Equals(tcpClient))
                    {
                        NetworkStream stream = c.GetStream();
                        stream.Write(message, 0, message.Length);
                    }
                }
                mut.ReleaseMutex();
                break;
            }
            return;
		}
    }//end of WorkerSender
}