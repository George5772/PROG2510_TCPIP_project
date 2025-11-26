using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
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
        public delegate void SetTextCallback(object obj);
        public static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        public static Int32 port = 13000;
        private TcpClient senderClient;
        private TcpClient receiverClient;
        private StreamWriter senderWriter;

        public CommunicationsWindow()
        {
            InitializeComponent();
            Receiving.MsgUpdated += ReceivedTextEventHandler;

            receiverClient = new TcpClient("127.0.0.1", port);
            ParameterizedThreadStart tReceiverStart = new ParameterizedThreadStart(Receiving.ReceiveMessages);
            Thread tReceiver = new Thread(tReceiverStart);
            tReceiver.Start(receiverClient);

            senderClient = new TcpClient("127.0.0.1", port);
            senderWriter = new StreamWriter(senderClient.GetStream(), System.Text.Encoding.ASCII);
            senderWriter.AutoFlush = true;
            senderWriter.WriteLine("Sender");
        }

        private void btnSendInput_Click(object sender, RoutedEventArgs e)
        {
            //send to server
            senderWriter.WriteLine(txtUserInput.Text);
            txtLogBox.Text += "Sent message to server\n";
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
            //get ip address with regex?
        }

        /// <summary>
        /// allows the user to set the server port to connect to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuConfigPort_Click(object sender, RoutedEventArgs e)
        {
            //get port with regex?
        }

        /// <summary>
        /// catches an event that the message has been updated and sends the text to the dispatcher function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivedTextEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if(Receiving.msg != null)
            {
                SetReceivedText(Receiving.msg);
            }
        }

        /// <summary>
        /// checks for access to the textbox, dispatches to the owner thread
        /// and owner thread writes to the textbox
        /// </summary>
        /// <param name="str"></param>
        private void SetReceivedText(object str)
        {
            System.Windows.Threading.Dispatcher dispatcher = txtUserInput.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                SetTextCallback callback = new SetTextCallback(SetReceivedText);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                if(Receiving.exception == true)
                {
                    txtLogBox.Text += "ERROR: " + (string)str + "\n";
                    Receiving.exception = false;
                }
                else if ((string)str != null && (string)str != string.Empty)
                {
                    if(((string)str).Equals("STOP"))
                    {
                        senderWriter.WriteLine(txtUserInput.Text);
                        txtLogBox.Text += "Server Shutting Down\n";
                        txtUserInput.IsEnabled = false;
                        btnSendInput.IsEnabled = false;
                    }
                    else
                    {
                        txtReceived.Text += (string)str + "\n";
                        txtLogBox.Text += "Message received from server\n";
                    }
                }
            }
        }
    }
}