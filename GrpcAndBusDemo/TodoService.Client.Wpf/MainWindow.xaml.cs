using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using NServiceBus;
using System;
using System.Linq;
using System.Windows;
using TodoService.Services;

namespace TodoService.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEndpointInstance endpointInstance;
        public MainWindow(IEndpointInstance endpointInstance)
        {
            this.endpointInstance = endpointInstance;
            InitializeComponent();
        }

        private void DisableButtons()
        {
            AddNewItem.IsEnabled = false;
            AddViaCommand.IsEnabled = false;
        }

        private void EnableButtons()
        {
            AddNewItem.IsEnabled = true;
            AddViaCommand.IsEnabled = true;
        }

        private async void AddNewItem_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            OutputTextBlock.Text = "calling service...";
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new TodoList.TodoListClient(channel);
            var reply = await client.AddAsync(
                              new AddRequest { 
                              Title = TitleTextBox.Text,
                              DueDate = Timestamp.FromDateTime((DueDateDatePicker.SelectedDate ?? DateTime.MaxValue).ToUniversalTime())
                              });
            OutputTextBlock.Text += $"Todo item with id {reply.Item.Id} created\n";
            EnableButtons();
        }

        private async void AddViaCommand_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            var id = Guid.NewGuid();
            OutputTextBlock.Text = $"Sending command with id {id}...";
            await endpointInstance.Send(new AddCommand {
                Id = id.ToString(),
                Title = TitleTextBox.Text,
                DueDate = Timestamp.FromDateTime((DueDateDatePicker.SelectedDate ?? DateTime.MaxValue).ToUniversalTime())
            }).ConfigureAwait(false);
            EnableButtons();
        }

        private async void GetAll_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new TodoList.TodoListClient(channel);
            var getAllReply = await client.GetAllAsync(new Services.GetAllRequest());

            OutputTextBlock.Text = $"All items:\n";
            OutputTextBlock.Text += string.Concat(getAllReply.Items.Select(i => $"{i.Id} - {i.Title} - {i.DueDate.ToDateTime()} - {i.IsCompleted}\n"));
            EnableButtons();
        }
    }
}
