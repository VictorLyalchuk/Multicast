using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Multicast_chat
{
    public partial class MainWindow : Window
    {
        private const string IpAddress = "192.168.0.255";
        private const int Port = 11000;

        private UdpClient udpClient;
        private IPAddress multicastAddress;
        private CancellationTokenSource cancelTokenSource;
        private CancellationToken cancelToken;
        private string userName;
        public MainWindow()
        {
            InitializeComponent();
            udpClient = new UdpClient(Port);
            multicastAddress = IPAddress.Parse(IpAddress);

            textBoxAddress.Text = multicastAddress.ToString();
            textBoxPort.Text = Port.ToString();
            cancelTokenSource = new CancellationTokenSource();
        }

        private async void joinChatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                userName = userNameBox.Text;
                joinChatButton.IsEnabled = false;
                ExitChatButton.IsEnabled = true;
                await Task.Run(() => Listening(cancelTokenSource.Token));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExitChatButton_Click(object sender, RoutedEventArgs e)
        {
            //Зупинка процесу       

            if (cancelToken != null && !cancelToken.IsCancellationRequested)
            {
                cancelTokenSource.Cancel();
            }

            udpClient.DropMulticastGroup(multicastAddress);
            udpClient.Close();
            joinChatButton.IsEnabled = true;
            ExitChatButton.IsEnabled = false;
        }
        private async void Listening(CancellationToken cancelTokenF)
        {
            cancelToken = cancelTokenF;

            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, Port);
            while (true)
            {
                //var result = udpClient.Receive(ref groupEP);
                var result = await udpClient.ReceiveAsync();
                string message = Encoding.Unicode.GetString(result.Buffer);
                Dispatcher.Invoke(() =>
                {
                    listChat.Items.Add(message);
                });
            }

        }

        private void messageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string? message = messageBox.Text.ToString(); //повідомлення для відправки

            SendMessageAsync(message);
        }
        private async void SendMessageAsync(string message)
        {
            // иначе добавляем к сообщению имя пользователя
            message = $"{userName}: {message}";
            byte[] data = Encoding.Unicode.GetBytes(message);
            // и отправляем в группу
            await udpClient.SendAsync(data, data.Length, new IPEndPoint(multicastAddress, Port));
        }
    }
}
