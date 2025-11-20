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
        /// <summary>
        /// takes the message from the client and sends it to all other clients
        /// </summary>
        public static void SendMessage(object? client)
        {
            StreamReader? sr = (StreamReader?)client;
            if (sr != null)
            {
                while(true)
                {
                    Mutex? mut;
                    string? message = sr.ReadLine();

                    if (message == null)
                    {
                        message = "ERROR";
                    }

                    Console.WriteLine(message);

                    if (!Mutex.TryOpenExisting("A07Mutex", out mut))
                    {
                        mut = new Mutex(true, "A07Mutex");
                        mut.ReleaseMutex();
                    }
                    mut.WaitOne();

                    //compare client to client list and send message to other clients
                    foreach (StreamWriter sw in RunServer.ClientWriters)
                    {
                        sw.WriteLine(message);
                        //stream.Write(message, 0, message.Length);
                    }

                    mut.ReleaseMutex();
                }
                
            }

            return;
        }
    }//end of WorkerSender
}