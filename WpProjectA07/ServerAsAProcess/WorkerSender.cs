using System.Net.Http;
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
		public static volatile bool StopLoop = false;
		/// <summary>
		/// takes the message from the client and sends it to all other clients
		/// </summary>
		public static void SendMessage(object? client)
		{
			//cast parameter
			StreamReader? sr = (StreamReader?)client;

			if (sr != null)
			{
				try
				{
					while (!StopLoop)
					{
						//get client message to send
						string? message = sr.ReadLine();

						if (message == null)
						{
							message = "ERROR";
						}

                        //Console.WriteLine(message);

                        //open mutex
                        Mutex? mut;
                        if (!Mutex.TryOpenExisting("A07Mutex", out mut))
						{
							mut = new Mutex(true, "A07Mutex");
							mut.ReleaseMutex();
						}

						//get mutex to access list
						mut.WaitOne();

						//send message to all receiver clients
						foreach (StreamWriter sw in RunServer.ClientWriters)
						{
							sw.WriteLine(message);
						}

						mut.ReleaseMutex();
					}//end of while
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());//change to logger
				}
				finally
				{
					//close the message reader
					sr.Close();
				}
			}//end of if
			return;
		}//end of method

	}//end of WorkerSender
}