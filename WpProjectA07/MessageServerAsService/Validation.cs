using System;
using System.Net;

namespace MessageServerAsService
{
    public static class Validation
    {
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
