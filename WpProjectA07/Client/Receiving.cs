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

        public static void ReceiveMessages(object? client)
		{
			try
			{
				TcpClient? tcpClient = (TcpClient?)client;
				if (tcpClient == null)
				{
					return;
				}
				NetworkStream? stream = tcpClient.GetStream();
				StreamReader streamReader = new StreamReader(stream, System.Text.Encoding.ASCII);
				StreamWriter streamWriter = new StreamWriter(stream, System.Text.Encoding.ASCII);
				streamWriter.AutoFlush = true;

				//tell server this is a receiver
				streamWriter.WriteLine("Receiver");

				while (true)
				{
					string? receivedMsg = streamReader.ReadLine();
					if (receivedMsg == null)
					{
						break;
					}
					msg = receivedMsg;
					MsgUpdated?.Invoke(null, new PropertyChangedEventArgs(nameof(msg)));
				}

                //string? data = "";
                //Byte[] bytes = new Byte[256];
                //int i;

                //while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                //{
                //    data = data + System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                //}
				//msg = data;
            }
			catch(Exception ex)
			{

			}
		}
    }//end of Receiving
}