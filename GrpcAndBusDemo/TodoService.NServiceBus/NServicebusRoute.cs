using System;

namespace TodoService.NServiceBus
{
    public class NServicebusRoute
    {
        public Type Type { get; private set; }
        public string Destination { get; private set; }

        public NServicebusRoute(Type type, string destination)
        {
            Type = type;
            Destination = destination;
        }

        public static implicit operator NServicebusRoute((Type type, string destination) tuple)
        {
            return new NServicebusRoute(tuple.type, tuple.destination);
        }

    }
}
