using System.Net.Http;
using System.Net.Sockets;
using System.Text;

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
		public static volatile bool StopLoop = false;
		/// <summary>
		/// takes the message from the client and sends it to all other clients
		/// </summary>
		public static void SendMessage(object? clientObj)
		{
			//cast parameter
			TcpClient? client = (TcpClient?)clientObj;

			if (client != null)
			{
				try
				{
					while (!StopLoop)
					{
						string? message = WorkerReceiver.getMessageFromClient(client);
						Console.WriteLine(message);
						if (message != null)
                        {
                            //open mutex
                            Mutex? mut;
                            if (!Mutex.TryOpenExisting("A07ClientMutex", out mut))
                            {
                                mut = new Mutex(true, "A07ClientMutex");
                                mut.ReleaseMutex();
                            }

                            //get mutex to access list
                            mut.WaitOne();

                            //send message to all receiver clients
                            foreach (TcpClient clientReceiver in RunServer.Clients)
                            {
								sendMessageToClient(clientReceiver, message);
                            }

                            mut.ReleaseMutex();
                        }
					}//end of while
				}
				catch (Exception ex)
                {
                    Logger.LogDataToFile(RunServer.Filepath, LoggerActions.Error, ex.Message);
                }
				finally
				{
					//close the client
					client.Close();
				}
			}//end of if
			return;
		}//end of method


        public static void sendMessageToClient(TcpClient client, string message)
        {
            try
            {
				NetworkStream stream = client.GetStream();
				using (StreamWriter sw = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogDataToFile(RunServer.Filepath, LoggerActions.Error, ex.Message);
            }
            return;
        }//end of method
    }//end of WorkerSender
}