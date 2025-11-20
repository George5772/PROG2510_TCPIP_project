using System.Net;
using System.Net.Sockets;
using System.Text;

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

            try
            {
                //set ip and port
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                Int32 port = 13000;

                //make tcplistener
                server = new TcpListener(localAddr, port);

                //define threads
                ParameterizedThreadStart tStart = new ParameterizedThreadStart(WorkerSender.SendMessage);

                //start server
                server.Start();

                while (!stop)
                {
                    //connect to client
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    StreamReader sr = new StreamReader(stream);

                    if (client == null)
                    {
                        continue;
                    }

                    string? firstMessage = sr.ReadLine();

                    if (firstMessage == null)
                    {
                        continue;
                    }
                    Console.WriteLine(firstMessage);

                    if (firstMessage.Equals("Receiver"))
                    {
                        //add receiving client to list
                        StreamWriter sw = new StreamWriter(stream);
                        sw.AutoFlush = true;
                        mut.WaitOne();
                        RunServer.ClientWriters.Add(sw);
                        mut.ReleaseMutex();
                    }
                    else if (firstMessage.Equals("Sender"))
                    {
                        //send client to sender thread
                        Thread t = new Thread(tStart);
                        t.Start(sr);
                        RunServer.Threads.Add(t);
                    }
                }

                foreach (Thread t in RunServer.Threads)
                {
                    t.Join();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return;
        }
    }//end of WorkerReceiver
}