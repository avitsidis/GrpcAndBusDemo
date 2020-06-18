using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
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

namespace TodoService.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void AddNewItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem.IsEnabled = false;
            OutputTextBlock.Text = "calling service...";
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Services.TodoList.TodoListClient(channel);
            var reply = await client.AddAsync(
                              new Services.AddRequest { 
                              Title = TitleTextBox.Text,
                              DueDate = Timestamp.FromDateTime((DueDateDatePicker.SelectedDate ?? DateTime.MaxValue).ToUniversalTime())
                              });
            OutputTextBlock.Text += $"Todo item with id {reply.Item.Id} created\n";
            var getAllReply = await client.GetAllAsync(new Services.GetAllRequest());

            OutputTextBlock.Text += $"All items:\n";
            OutputTextBlock.Text += string.Concat(getAllReply.Items.Select(i => $"{i.Id} - {i.Title} - {i.DueDate.ToDateTime()} - {i.IsCompleted}\n"));
            AddNewItem.IsEnabled = true;
        }
    }
}
