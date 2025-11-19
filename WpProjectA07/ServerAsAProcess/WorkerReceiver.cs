using System.Net;
using System.Net.Sockets;

namespace ServerAsAProcess
{
    /*
	* FILE : WorkerReceiver.cs
	* PROJECT : PROG2510 - A07
	* PROGRAMMER : George Shapka
	* FIRST VERSION : 11/18/2025 12:04:36 PM
	*/

    /*
	* NAME : WorkerReceiver
	* PURPOSE : holds methods for the server threads responsible for receiving messages
	*/
    internal class WorkerReceiver
    {
		public static bool stop = false;
		public static int numOfThreads = 0;
		public static int MaxNumOfThreads = 2;



		/// <summary>
		/// receives a message from a tcp/ip client, and sends it to another thread
		/// </summary>
		public static void ReceiveMessage()
		{
			TcpListener? server = null;
			Mutex? mut;
            if (!Mutex.TryOpenExisting("A07Mutex", out mut))
            {
                mut = new Mutex(true, "A07Mutex");
                mut.ReleaseMutex();
            }

            if (!Mutex.TryOpenExisting("A07Mutex", out mut))
			{
				mut = new Mutex(true, "A07Mutex");
					mut.ReleaseMutex();
			}
			try
			{
				//set ip and port
				IPAddress localAddr = IPAddress.Parse("127.0.0.1");
				Int32 port = 13000;

				//make tcplistener
				server = new TcpListener(localAddr, port);

                //define threads
                ParameterizedThreadStart tStart = new ParameterizedThreadStart(WorkerSender.SendMessage);
                Thread t = new Thread(tStart);
                ParameterizedThreadStart tJoinerStart = new ParameterizedThreadStart(ThreadJoiner);
                Thread tJoiner = new Thread(tJoinerStart);

                //start server
                server.Start();

				while (!stop)
				{
					//wait for threads to finish
					while(numOfThreads == MaxNumOfThreads)
					{
						tJoiner.Join();
						numOfThreads--;
					}

                    //connect to client
                    TcpClient client = server.AcceptTcpClient();


					//add client to list
					mut.WaitOne();
                    RunServer.Clients.Add(client);
					mut.ReleaseMutex();

					//send client to sender thread
                    t.Start(client);
					numOfThreads++;
                }
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return;
		}
		


		/// <summary>
		/// waits for the specified thread to join
		/// </summary>
		/// <param name="t">Thread to be joined</param>
		public static void ThreadJoiner(object? t)
		{
			Thread? t1 = (Thread?)t;
			if(t1 != null)
            {
                t1.Join();
            }
		}

		public static string GetStringFromNetworkStream(TcpClient client)
		{
			NetworkStream stream = client.GetStream();
			string? data = "";
            Byte[] bytes = new Byte[256];
			int i;

			while((i = stream.Read(bytes, 0, bytes.Length)) != 0)
			{
				data = data + System.Text.Encoding.ASCII.GetString(bytes, 0, i);
			}
			return data;
        }
    }//end of WorkerReceiver
}