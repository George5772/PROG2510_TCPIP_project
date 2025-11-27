using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Timers;
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
	 * PROGRAMMER : George Shapka, Le Hai Quy Bui
	 * FIRST VERSION : 11/14/2025
	 */


    //load default port and ip from config file




    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CommunicationsWindow : Window
    {
        public delegate void SetTextCallback(object obj);
        private TcpClient? senderClient;
        private TcpClient? receiverClient;
        private StreamWriter? senderWriter;

        public static volatile string userId = new Random(DateTime.Now.Millisecond * DateTime.Now.Second).NextInt64().ToString();

        public CommunicationsWindow()
        {
            InitializeComponent();
            Receiving.MsgUpdated += ReceivedTextEventHandler;
            ipAddressTxtBox.Text = "10.144.97.215";
            porTxtBox.Text = "13000";
            //by default the client cannot send without connecting to a server first
            txtUserInput.IsEnabled = false;
            btnSendInput.IsEnabled = false;
        }



        /// <summary>
        /// sends a message to the server. if it cannot for any reason, close all connections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(senderWriter != null)
                {
                    senderWriter.WriteLine("MSG|" + userId + "|" + txtUserInput.Text);
                    txtLogBox.Text += "Sent message to server\n";
                    txtUserInput.Clear();
                }
            }
            catch (Exception ex)
            {
                txtLogBox.AppendText("ERROR: " + ex.Message);
                txtUserInput.IsEnabled = false;
                btnSendInput.IsEnabled = false;

                //close all connections
                if (senderClient != null)
                {
                    senderClient.Close();
                }
                if(receiverClient != null)
                {
                    receiverClient.Close();
                }
                if(senderWriter != null)
                {
                    senderWriter.Close();
                }
            }
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
            About about = new About();
            about.ShowDialog();
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
            //check if owner thread
            if (!dispatcher.CheckAccess())
            {
                //send to owner
                SetTextCallback callback = new SetTextCallback(SetReceivedText);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                //if an exception occured
                if (Receiving.exception == true)
                {
                    txtLogBox.Text += "ERROR: " + (string)str + "\n";
                    Receiving.exception = false;
                }
                //if the string has content
                else if ((string)str != null && (string)str != string.Empty)
                {
                    string[] messageParts = ((string)str).Split("|");

                    string messageType = messageParts[0];

                    //initiate server stopping
                    if (messageType.Equals("STOP"))
                    {
                        if (senderWriter != null)
                        {
                            //unblock the server
                            senderWriter.WriteLine("Server Shutdown");
                            txtLogBox.Text += "Server Shutting Down\n";

                            //disable user sending stuff
                            txtUserInput.IsEnabled = false;
                            btnSendInput.IsEnabled = false;
                        }
                    }
                    else if (messageType.Equals("MSG"))
                    {
                        if (messageParts.Length >= 3)
                        {
                            // from server: MSG|senderName|msgBody
                            string senderName = messageParts[1];
                            string msgBody = messageParts[2];

                            if (senderName.Equals(userId))
                            {
                                txtLogBox.Text += "Own message received from server\n";
                            }
                            else
                            {
                                txtReceived.Text += senderName + ": " + msgBody + "\n";
                                txtLogBox.Text += "Message received from server\n";

                                // send ACK back to server: ACK|receiverId|senderId|message
                                if (senderWriter != null)
                                {
                                    string ackMessage =
                                        "ACK|" + userId + "|" + senderName + "|" + msgBody;
                                    senderWriter.WriteLine(ackMessage);
                                }
                            }
                        }
                    }
                    else if (messageType.Equals("ACK"))
                    {
                        txtLogBox.Text += "ACK received\n";
                    }
                    else if (messageType.Equals(userId))
                    {
                        //do nothing
                        txtLogBox.Text += "Own message received from server\n";
                    }
                    //write the message (fallback old format)
                    else if (messageParts.Length >= 2)
                    {
                        string name = messageParts[0];
                        string message = messageParts[1];
                        txtReceived.Text += name + ": " + message + "\n";
                        txtLogBox.Text += "Message received from server\n";
                    }
                }
            }
        }



        /// <summary>
        /// trys to connect to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            if(!(ipAddressTxtBox.Text == string.Empty || nameTxtBox.Text == string.Empty || porTxtBox.Text == string.Empty))
            {
                try
                {
                    receiverClient = new TcpClient(ipAddressTxtBox.Text, Validation.validatePort(porTxtBox.Text));
                    ParameterizedThreadStart tReceiverStart = new ParameterizedThreadStart(Receiving.ReceiveMessages);
                    Thread tReceiver = new Thread(tReceiverStart);
                    tReceiver.Start(receiverClient);

                    senderClient = new TcpClient(ipAddressTxtBox.Text, Validation.validatePort(porTxtBox.Text));
                    senderWriter = new StreamWriter(senderClient.GetStream(), System.Text.Encoding.ASCII);
                    senderWriter.AutoFlush = true;
                    senderWriter.WriteLine("Sender");

                    txtUserInput.IsEnabled = true;
                    btnSendInput.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    txtLogBox.AppendText("ERROR: " + ex.Message);
                }
            }
            
        }//end of method

        private void txtReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtReceived.ScrollToEnd();
        }

        private void txtLogBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtLogBox.ScrollToEnd();
        }
    }
}