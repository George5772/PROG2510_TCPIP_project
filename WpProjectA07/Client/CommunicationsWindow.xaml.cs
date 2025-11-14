using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client
{
    /*
	 * FILE : CommunicationsWindow.xaml.cs
	 * PROJECT : PROG2510 - A07
	 * PROGRAMMER : George Shapka, Key
	 * FIRST VERSION : 11/14/2025
	 */


    //load default port and ip from config file




    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CommunicationsWindow : Window
    {
        public delegate void SetReceivedTextDelegate(string str);

        public CommunicationsWindow()
        {
            InitializeComponent();
            Receiving.MsgUpdated += ReceivedTextEventHandler;
        }

        private void btnSendInput_Click(object sender, RoutedEventArgs e)
        {
            //send to server
        }

        /// <summary>
        /// clears the text received text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearReceived_Click(object sender, RoutedEventArgs e)
        {
            txtReceived.Text = new string(string.Empty);
        }

        /// <summary>
        /// shows user about box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            //create about window
        }

        /// <summary>
        /// allows the user to set the servers ip address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuConfigIp_Click(object sender, RoutedEventArgs e)
        {
            //get ip address with regex
        }

        /// <summary>
        /// allows the user to set the server port to connect to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuConfigPort_Click(object sender, RoutedEventArgs e)
        {
            //get port with regex
        }

        /// <summary>
        /// catches an event that the message has been updated and updated the message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivedTextEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if(Receiving.msg != null && Receiving.msg != string.Empty)
            {
                txtReceived.Text += "\n" + Receiving.msg;
            }
        }

    }
}