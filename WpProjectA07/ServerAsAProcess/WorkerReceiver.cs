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

            try
            {
                //open mutex
                Mutex? mut;
                if (!Mutex.TryOpenExisting("A07Mutex", out mut))
                {
                    mut = new Mutex(true, "A07Mutex");
                    mut.ReleaseMutex();
                }

                //set ip and port
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");//change to config file
                Int32 port = 13000;//change to config file

                //make tcplistener
                server = new TcpListener(localAddr, port);

                //define thread start
                ParameterizedThreadStart tStart = new ParameterizedThreadStart(WorkerSender.SendMessage);

                //start server
                server.Start();

                while (!stop)
                {
                    //connect to client
                    TcpClient client = server.AcceptTcpClient();
                    if (client == null)
                    {
                        continue;
                    }
                    
                    //get stream
                    NetworkStream stream = client.GetStream();
                    StreamReader sr = new StreamReader(stream);

                    //read the first message from the client to see if it sends or receives messages
                    string? firstMessage = sr.ReadLine();
                    if (firstMessage == null)
                    {
                        continue;
                    }
                    //Console.WriteLine(firstMessage);

                    if (firstMessage.Equals("Receiver"))
                    {
                        //add a stream writer for the receiving client to the list
                        StreamWriter sw = new StreamWriter(stream);
                        sw.AutoFlush = true;

                        //get access to the list
                        mut.WaitOne();
                        RunServer.ClientWriters.Add(sw);
                        mut.ReleaseMutex();
                    }
                    else if (firstMessage.Equals("Sender"))
                    {
                        //send client to sender thread
                        Thread t = new Thread(tStart);
                        t.Start(sr);

                        //add thread to the sender list
                        RunServer.Threads.Add(t);
                    }
                    else if(firstMessage.Equals("Stop"))
                    {
                        //signals the threads to leave
                        WorkerSender.StopLoop = true;

                        //close stream
                        sr.Close();
                        break;
                    }
                }//end of while loop

                //join sender threads
                foreach (Thread t in RunServer.Threads)
                {
                    t.Join();
                }

                //close client receiver writers
                foreach(StreamWriter sw in RunServer.ClientWriters)
                {
                    sw.Close();
                }
            }//end of try
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return;
        }//end of method

    }//end of WorkerReceiver
}