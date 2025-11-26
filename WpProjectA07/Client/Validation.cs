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
