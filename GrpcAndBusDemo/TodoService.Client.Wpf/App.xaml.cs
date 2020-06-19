using NServiceBus;
using NServiceBus.ProtoBufGoogle;
using System.Windows;
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
            var endpointConfiguration = GetEndpointConfiguration();

            endpointInstance = await Endpoint.Start(endpointConfiguration);
            MainWindow mainWindow = new MainWindow(endpointInstance);
            mainWindow.Show();
        }

        private static EndpointConfiguration GetEndpointConfiguration()
        {
            var endpointConfiguration = new EndpointConfiguration("Client.Wpf");
            endpointConfiguration.SendOnly();
            endpointConfiguration.UseSerialization<ProtoBufGoogleSerializer>();
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(type => type.Name.EndsWith("Event"));

            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(AddCommand), "TodoService");
            
            return endpointConfiguration;
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await endpointInstance.Stop();
            base.OnExit(e);
        }
    }
}
