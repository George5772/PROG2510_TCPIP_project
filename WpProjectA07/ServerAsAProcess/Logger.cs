using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAsAProcess
{
	/*
	 * FILE : Logger.cs
	 * PROJECT : PROG2510 A06
	 * PROGRAMMER : George Shapka
	 * FIRST VERSION : 11/11/2025 11:34:00 PM
	 */
	internal class Logger
	{
		/// <summary>
		/// logs data to a file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="action"></param>
		/// <param name="message"></param>
		public static void LogDataToFile(string filePath, string action, string message)
		{
			FileStream file;
			StreamWriter sw;
			try
			{
				using (file = new FileStream(filePath, FileMode.Append))
				{
					using (sw = new StreamWriter(file))
					{
						sw.WriteLine("[" + action + "][" + message + $"][{DateTime.Now.ToString()}]");
					}
				}
			}//end of try
			catch
			{
				
			}//end of catch

		}//end of LogDataToFile

	}//end of Logger

}//end of A06Service