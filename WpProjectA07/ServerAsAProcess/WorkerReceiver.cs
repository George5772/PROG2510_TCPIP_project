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
			while (!stop)
			{
				//connect client

				//add client to list

				//send client to sender thread
			}
			return;
		}
    }//end of WorkerReceiver
}