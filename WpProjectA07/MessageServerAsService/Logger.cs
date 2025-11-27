using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageServerAsService
{
    /*
	 * FILE : Logger.cs
	 * PROJECT : PROG2510 A06
	 * PROGRAMMER : George Shapka, Le Hai Quy Bui
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
                string directory = Path.GetDirectoryName(filePath);

                // Ensure directory exists
                if (directory != null && directory != string.Empty && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (file = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (sw = new StreamWriter(file))
                    {
                        sw.WriteLine("[" + action + "][" + message + $"][{DateTime.Now.ToString()}]");
                    }
                }
            }
            catch
            {
                // ignored on purpose to avoid service crash
            }

            return;
        }

    }//end of LogDataToFile

}//end of Logger

