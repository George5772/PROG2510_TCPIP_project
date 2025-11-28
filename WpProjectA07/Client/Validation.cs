/*
* FILE : Validation.cs
* PROJECT : PROG2510 - A07
* PROGRAMMER : George Shapka, Le Hai Quy Bui
* FIRST VERSION : 11/14/2025
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public static class Validation
    {
        /// <summary>
        /// method to validate the ipAddress in correct format
        /// </summary>
        /// <param name="ipAddress"> ipadress need to format</param>
        /// <returns></returns>
        public static IPAddress validateIPFormat(string ipAddress)
        {
            IPAddress res;
            if (!IPAddress.TryParse(ipAddress, out res))
            {
                res = null;
                MessageBox.Show("Incorrect Format Of IP Addresss\n");
            }
            return res;
        }
        /// <summary>
        /// method to validate the portNumber in correct format
        /// </summary>
        /// <param name="portNumber"> portNumber need to format</param>
        /// <returns></returns>
        public static Int32 validatePort(string portNumber)
        {
            Int32 res;
            if (!Int32.TryParse(portNumber, out res))
            {
                res = -1;
                MessageBox.Show(" Invalid Format Of Port Number\n");
            }
            return res;
        }
    }
}
