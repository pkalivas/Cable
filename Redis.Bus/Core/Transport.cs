using System.Reflection;
using Redis.Bus.Interfaces;

namespace Redis.Bus.Core;

public class Transport
{
    private readonly Dictionary<string, List<Type>> _messageHandlers;

    public Transport(AppDomain currentDomain)
    {
        _messageHandlers = currentDomain.GetAssemblies()
            .SelectMany(assembly => GetAllImplementingTypes(typeof(IMessageHandler<>), assembly))
            .DistinctBy(member => member.AssemblyQualifiedName)
            .SelectMany(handler => handler.GetInterfaces()
                .Where(hand => hand.IsGenericType && hand.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                .SelectMany(hand => hand.GetGenericArguments()
                    .Select(arg => new
                    {
                        Handler = handler,
                        MessageType = arg.Name,
                    })))
            .GroupBy(val => val.MessageType)
            .ToDictionary(key => key.Key, value => value.Select(val => val.Handler).ToList());
    }
    
    public async Task Publish<T>(T message) where T : IMessage
    {
        var messageType = message.GetType();

        foreach (var handlerType in _messageHandlers[messageType.Name])
        {
            var handler = Activator.CreateInstance(handlerType);
            var method = handlerType.GetMethod("Handle", new[] { messageType });
            await (Task) method.Invoke(handler, new object[] { message });   
        }
    }

    private static IEnumerable<Type> GetAllImplementingTypes(Type openGenericType, Assembly assembly) =>
        assembly.GetTypes()
            .Where(type => type.GetInterfaces()
                .Any(inter => inter.IsGenericType && openGenericType.IsAssignableFrom(inter.GetGenericTypeDefinition())))
            .ToList();
}


// _messageTypes = _handlerTypes.Values
//     .SelectMany(handler => handler.GetInterfaces()
//         .Where(hand => hand.IsGenericType && hand.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
//         .Select(hand => hand.GetGenericArguments())
//         .Where(hand => hand.Any())
//         .Select(hand => hand.First()))
//     .DistinctBy(handler => handler.AssemblyQualifiedName)
//     .ToDictionary(key => key.Name);
