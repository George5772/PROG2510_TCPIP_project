using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageServerAsService
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
        public static void SendMessage(object clientObj)
        {
            //cast parameter
            TcpClient client = (TcpClient)clientObj;
            

            if (client != null)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    while (!StopLoop)
                    {
                        RunServer.OkayToContinue.WaitOne();
                        if (!stream.DataAvailable)
                        {
                            Thread.Sleep(50); 
                            continue;         
                        }
                        string message = WorkerReceiver.getMessageFromClient(client);
                        Console.WriteLine(message);
                        if (message != null)
                        {
                            //open mutex
                            Mutex mut;
                            if (!Mutex.TryOpenExisting("A07ClientMutex", out mut))
                            {
                                mut = new Mutex(true, "A07ClientMutex");
                                mut.ReleaseMutex();
                            }
                            string[] parts = message.Split('|');
                            string type = parts[0];

                            //get mutex to access list
                            mut.WaitOne();

                            // MSG|senderId|msgBody|senderName
                            if (type.Equals("MSG") && parts.Length >= 4)
                            {
                                string senderId = parts[1];
                                string msgBody = parts[2];
                                string senderName = parts[3];

                                // ACK back to sender 
                                string ackToSender = "ACK|SERVER|" + senderId + "|" + msgBody;
                                sendMessageToClient(client, ackToSender);

                                // broadcast message to all receiver clients
                                int i = 0;
                                while (i < RunServer.Clients.Count)
                                {
                                    TcpClient clientReceiver = RunServer.Clients[i];

                                    if (clientReceiver.Connected)
                                    {
                                        string messageToReceiver = "MSG|" + senderId + "|" + msgBody + "|" + senderName;
                                        sendMessageToClient(clientReceiver, messageToReceiver);
                                    }

                                    i++;
                                }
                            }
                            // ACK|receiverId|msgBody  
                            else if (type.Equals("ACK") && parts.Length >= 3)
                            {
                                string receiverId = parts[1];
                                string msgBody = parts[2];

                                string logLine = "ACK from " + receiverId + " : " + msgBody;
                                Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.Message, logLine);

                                // broadcast ACK to all receiver clients
                                int i = 0;
                                while (i < RunServer.Clients.Count)
                                {
                                    TcpClient clientReceiver = RunServer.Clients[i];

                                    if (clientReceiver.Connected)
                                    {
                                        string ackBroadcast = "ACK|" + receiverId + "|" + msgBody;
                                        sendMessageToClient(clientReceiver, ackBroadcast);
                                    }

                                    i++;
                                }
                            }
                            mut.ReleaseMutex();

                        }
                        else
                        {
                            break; // Exit loop if client disconnects
                        }
                    }//end of while
                }
                catch (Exception ex)
                {
                    Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.Error, ex.Message);
                    EventLogger.Log(LoggerActions.Error + ex.Message);
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
                StreamWriter sw = new StreamWriter(stream, Encoding.ASCII, 1024, true);
                sw.AutoFlush = true;
                sw.WriteLine(message);
            }
            catch (Exception ex)
            {
                Logger.LogDataToFile(RunServer.LogFilePath, LoggerActions.Error, ex.Message);
                EventLogger.Log(LoggerActions.Error + ex.Message);
            }
            return;
        }//end of method
    }//end of WorkerSender
}
