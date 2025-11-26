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

                            // MSG|senderId|text
                            if (type.Equals("MSG") && parts.Length >= 3)
                            {
                                string senderId = parts[1];
                                string msgBody = parts[2];

                                // 1. ACK back to sender
                                string ackToSender =
                                    "ACK|SERVER|" + senderId + "|" + msgBody;
                                sendMessageToClient(client, ackToSender);

                                // 2. broadcast to all receivers
                                int i = 0;
                                while (i < RunServer.Clients.Count)
                                {
                                    TcpClient clientReceiver = RunServer.Clients[i];

                                    if (clientReceiver.Connected)
                                    {
                                        string messageToReceiver =
                                            "MSG|" + senderId + "|" + msgBody;
                                        sendMessageToClient(clientReceiver, messageToReceiver);
                                    }

                                    i++;
                                }
                            }
                            // ACK|receiverId|senderId|text
                            else if (type.Equals("ACK") && parts.Length >= 4)
                            {
                                string receiverId = parts[1];
                                string senderId = parts[2];
                                string msgBody = parts[3];

                                string logLine =
                                    "ACK from " + receiverId +
                                    " to " + senderId +
                                    " : " + msgBody;
                                Logger.LogDataToFile(
                                    RunServer.LogFilePath,
                                    LoggerActions.Message,
                                    logLine);

                                // forward ACK to original sender's receiver
                                int j = 0;
                                bool found = false;
                                while (j < RunServer.ClientUserIds.Count && !found)
                                {
                                    if (RunServer.ClientUserIds[j] == senderId)
                                    {
                                        TcpClient senderReceiver = RunServer.Clients[j];
                                        string ackForward =
                                            "ACK|" + receiverId + "|" + senderId + "|" + msgBody;
                                        sendMessageToClient(senderReceiver, ackForward);
                                        found = true;
                                    }
                                    j++;
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
    }//end of WorkerSen
}
