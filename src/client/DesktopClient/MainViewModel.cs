using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DesktopClient
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        HubConnection connection;


        private ObservableCollection<OrderDto> _taxiOrders;

        public ObservableCollection<OrderDto> TaxiOrder
        {
            get { return _taxiOrders; }
            set { _taxiOrders = value; NotifyPropertyChanged(); }
        }



        public MainViewModel()
        {
            TaxiOrder = new ObservableCollection<OrderDto>();
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7104/taxiHub", options=>{
                    options
                })
                .Build();
            connection.On<TaxiOrder>("NewOrder", handler);
            try
            {
                Task.Run(async () =>
                await connection.StartAsync());
                Debug.WriteLine("Common notify started");
                //sendBtn.IsEnabled = true;
                //chatbox.Items.Add("Вы подключились к системе такси");

            }
            catch (Exception ex)
            {

                Debug.WriteLine("" + ex.Message);
            }
        }

        private void handler(TaxiOrder order)
        {
            Task.Run(() =>
                App.Current.Dispatcher.Invoke(() => TaxiOrder.Add(new OrderDto()
                {
                    Id = order.OrderId,
                    ClientName = order.ClientName,
                    AcceptOrder = new RelayCommand(async o =>
                    {
                        using var client = new HttpClient();

                        OrderAcceptDto accept = new OrderAcceptDto();
                        accept.DriverId = order.ClientId;
                        accept.OrderId = "ozX1TqFBy7jlpHtlhzA0vw"; // change it to
                        //var json = 
                        var json = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

                        var request = await client.PostAsJsonAsync("https://localhost:7104/api/v1/taxidriver/order/accept", accept);
                        var response = await request.Content.ReadAsStringAsync();
                        MessageBox.Show("order accepted 123");

                    })
                }
               )));
        }
    }
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public RelayCommand? AcceptOrder { get; set; }
    }
    public class OrderAcceptDto
    {
        [Required]
        public Guid DriverId { get; set; }
        [Required]
        public string OrderId { get; set; }
    }
}
