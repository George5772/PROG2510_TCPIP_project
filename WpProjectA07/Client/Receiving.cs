using System.ComponentModel;

namespace Client
{
    /*
	* FILE : Receiving.cs
	* PROJECT : PROG2510 - A07
	* PROGRAMMER : George Shapka
	* FIRST VERSION : 11/14/2025 9:02:33 AM
	*/

    /*
	* NAME : Receiving
	* PURPOSE :
	*/
    internal class Receiving
    {
        public static event PropertyChangedEventHandler? MsgUpdated;
		public static string? msg;
    }//end of Receiving
}