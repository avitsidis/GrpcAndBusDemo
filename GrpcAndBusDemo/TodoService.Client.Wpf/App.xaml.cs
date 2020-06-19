using NServiceBus;
using System.Windows;
using TodoService.NServiceBus;
using TodoService.Services;

namespace TodoService.Client.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IEndpointInstance endpointInstance;

        private async void AppOnStartup(object sender, StartupEventArgs e)
        {
            var endpointConfiguration = NServiceBusHelper.GetSendOnlyEndpointConfiguration("Client.Wpf",new NServicebusRoute[] { 
                (typeof(AddCommand), "TodoService")
            });

            endpointInstance = await Endpoint.Start(endpointConfiguration);
            MainWindow mainWindow = new MainWindow(endpointInstance);
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await endpointInstance.Stop();
            base.OnExit(e);
        }
    }
}
