using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;

namespace MessageServerAsService
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
        public static volatile bool stop = false;
  


        /// <summary>
        /// receives a message from a tcp/ip client, and sends it to another thread
        /// </summary>
        public static void ReceiveMessage()
        {
            TcpListener server = null;

            try
            {
                //open mutex
                Mutex clientMut;
                if (!Mutex.TryOpenExisting("A07ClientMutex", out clientMut))
                {
                    clientMut = new Mutex(true, "A07ClientMutex");
                    clientMut.ReleaseMutex();
                }

                string ipAddress = ConfigurationManager.AppSettings["ipAddress"];
                string portNumber = ConfigurationManager.AppSettings["portNumber"];
                //set ip and port
                IPAddress localAddr = Validation.validateIPFormat(ipAddress);
                Int32 port = Validation.validatePort(portNumber);

                if (RunServer.localAddr == null || RunServer.port <= 0)
                {
                    // log and bail out gracefully
                    Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.Error, "Invalid IP address or port in configuration.");
                    EventLogger.Log(LoggerActions.Error + "Invalid IP address or port in configuration.");
                    
                }
                else
                {
                    //make tcplistener
                    server = new TcpListener(localAddr, port);

                    //define thread start
                    ParameterizedThreadStart tStart = new ParameterizedThreadStart(WorkerSender.SendMessage);

                    //start server
                    server.Start();

                    while (!stop)
                    {
                        RunServer.OkayToContinue.WaitOne();

                        //connect to client
                        TcpClient client = server.AcceptTcpClient();
                        if (client != null)
                        {
                            //get stream
                            string firstMessage = getMessageFromClient(client);

                            //read the first message from the client to see if it sends or receives messages

                            if (firstMessage != null)
                            {
                                string[] firstParts = firstMessage.Split('|');
                                string type = firstParts[0];
                                string userId = string.Empty;
                                if (firstParts.Length >= 2)
                                {
                                    userId = firstParts[1];
                                }

                                if (type.Equals("Receiver"))
                                {
                                    //add a client to the receiving client list
                                    clientMut.WaitOne();
                                    RunServer.Clients.Add(client);
                                    clientMut.ReleaseMutex();
                                }
                                else if (type.Equals("Sender"))
                                {
                                    //send client to sender thread
                                    Thread t = new Thread(tStart);
                                    t.Start(client);

                                    //add thread to the sender list
                                    clientMut.WaitOne();
                                    RunServer.Threads.Add(t);
                                    clientMut.ReleaseMutex();
                                }
                                else if (type.Equals("Stop"))
                                {
                                    //signals the threads to leave
                                    WorkerSender.StopLoop = true;
                                    stop = true;
                                }
                            }//end of message != null if
                        }//end of client != null if

                    }//end of while loop

                    foreach (TcpClient client in RunServer.Clients)
                    {
                        WorkerSender.sendMessageToClient(client, "STOP");
                    }

                    //join sender threads
                    foreach (Thread t in RunServer.Threads)
                    {
                        t.Join();
                    }

                    clientMut.WaitOne();
                    //close client receiver writers
                    foreach (TcpClient client in RunServer.Clients)
                    {
                        client.Close();
                    }
                    clientMut.ReleaseMutex();
                }
            }//end of try
            catch (Exception ex)
            {
                Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.Error, ex.Message);
                EventLogger.Log(LoggerActions.Error + ex.Message);
            }
            return;
        }//end of method



        /// <summary>
        /// gets a message sent by a tcp client
        /// </summary>
        /// <param name="client"></param>
        /// <returns>the message, or null</returns>
        public static string getMessageFromClient(TcpClient client)
        {
            StreamReader sr = null;
            string message = null;
            try
            {
                sr = new StreamReader(client.GetStream(), Encoding.ASCII, false, 1024, true);
                message = sr.ReadLine();
            }
            catch (Exception ex)
            {
                Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.Error, ex.Message);
                EventLogger.Log(LoggerActions.Error + ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return message;
        }//end of method

    }//end of WorkerReceiver
}
