using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    /*
	* FILE : Receiving.cs
	* PROJECT : PROG2510 - A07
	* PROGRAMMER : George Shapka
	* FIRST VERSION : 11/14/2025 9:02:33 AM
	*/

    /*
	* NAME : Receiving
	* PURPOSE :
	*/
    internal class Receiving
    {
        public static event PropertyChangedEventHandler? MsgUpdated;
		public static string? msg;
		public static volatile bool exception = false;

        public static void ReceiveMessages(object? client)
		{
			// make sure this clean before
            exception = false;
            try
			{
				TcpClient? tcpClient = (TcpClient?)client;
				if (tcpClient == null)
				{
					return;
				}
				NetworkStream networkStream = tcpClient.GetStream();
				StreamReader sr = new StreamReader(networkStream, System.Text.Encoding.ASCII);
				StreamWriter sw = new StreamWriter(networkStream, System.Text.Encoding.ASCII);
				sw.AutoFlush = true;

                //tell server this is a receiver
                sw.WriteLine("Receiver|" + CommunicationsWindow.userId);

                bool doLoop = true;
				while (doLoop)
				{
					string? receivedMsg = sr.ReadLine();
					if (receivedMsg != null)
                    {
                        msg = receivedMsg;
                        MsgUpdated?.Invoke(null, new PropertyChangedEventArgs(nameof(msg)));
                    }
				}
            }
			catch(Exception ex)
			{
				exception = true;
				msg = ex.Message;
			}
		}
    }//end of Receiving
}