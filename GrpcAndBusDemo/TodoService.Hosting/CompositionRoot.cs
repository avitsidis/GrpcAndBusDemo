using NServiceBus;
using NServiceBus.ProtoBufGoogle;
using NServiceBus.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using TodoService.Data.InMemory;
using TodoService.Domain;
using TodoService.Services;

namespace TodoService.Hosting
{
    public static class CompositionRoot
    {
        public static Container Container { get; private set; }
        static CompositionRoot()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            Container.Options.AllowOverridingRegistrations = true;
            Container.Options.AutoWirePropertiesImplicitly();

            RegisterNserviceBus();
            RegisterRepositories();
            RegisterGrpcServices();
        }

        private static void RegisterRepositories()
        {
            Container.RegisterInstance<ITodoItemRepository>(new InMemoryTodoItemRepository());
        }

        private static void RegisterGrpcServices()
        {
            Container.Register<TodoGrpcService>();
        }

        private static void RegisterNserviceBus()
        {
            var endpointConfiguration = new EndpointConfiguration("TodoService");
            endpointConfiguration.UseSerialization<ProtoBufGoogleSerializer>();
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(type => type.Name.EndsWith("Event"));

            endpointConfiguration.UseTransport<LearningTransport>();

            endpointConfiguration.UseContainer<SimpleInjectorBuilder>(
                customizations =>
                {
                    customizations.UseExistingContainer(Container);
                });
            Container.RegisterInstance(endpointConfiguration);
        }
    }
}
