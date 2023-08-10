using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7004/notify")
                .Build();
            connection.On<string>("Recieve", ( message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newNotify = $" {message}";
                    
                    chatbox.Items.Insert(0, newNotify);
                    Debug.WriteLine("Notify recieved -" + newNotify);
                });
            });
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.StartAsync();
                Debug.WriteLine("Common notify started");
                sendBtn.IsEnabled = true;
                chatbox.Items.Add("Вы вошли в чат");

            }
            catch (Exception ex)
            {

                Debug.WriteLine("" + ex.Message);
            }
        }

        private async void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.SendAsync("SendNotify", userTextBox.Text, messageTextBox.Text);
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
        }
    }
}
