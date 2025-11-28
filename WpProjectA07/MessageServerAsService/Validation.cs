using System;
using System.Net;

namespace MessageServerAsService
{
    public static class Validation
    {
        /// <summary>
        /// This method for validateIPFormat
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static IPAddress validateIPFormat(string ipAddress)
        {
            IPAddress res;
            bool parsed = IPAddress.TryParse(ipAddress, out res);
            if (!parsed)
            {
                res = null;
                EventLogger.Log("User provided invalid format of IP address.");
            }
            return res;
        }
        /// <summary>
        /// This method for validate port number
        /// </summary>
        /// <param name="portNumber"></param>
        /// <returns></returns>
        public static int validatePort(string portNumber)
        {
            int result;
            bool parsed = Int32.TryParse(portNumber, out result);
            if (!parsed)
            {
                result = -1;
                EventLogger.Log("User provided invalid format of port number.");
            }
            return result;
        }
    }
}
