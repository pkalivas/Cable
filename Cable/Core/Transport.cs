using System.Reflection;
using Cable.Interfaces;

namespace Cable.Core;

public class Transport
{
    private readonly Dictionary<Type, List<object>> _messageHandlerInstances = new();

    public Transport(AppDomain currentDomain)
    {
        var messageHandlers = currentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetInterfaces()
                .Any(inter => inter.IsGenericType && typeof(IMessageHandler<>).IsAssignableFrom(inter.GetGenericTypeDefinition())))
            .ToList();
        
        var handlerInstances = messageHandlers
            .Select(Activator.CreateInstance)
            .ToList();

        foreach (var handler in handlerInstances)
        {
            var handlerType = handler.GetType();
            var messageTypes = handlerType.GetInterfaces()
                .Where(hand => hand.IsGenericType && hand.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                .SelectMany(hand => hand.GetGenericArguments()
                    .Select(arg => new
                    {
                        Handler = handler,
                        MessageType = arg,
                    }))
                .GroupBy(val => val.MessageType)
                .ToList();
            
            foreach (var messageType in messageTypes)
            {
                if (!_messageHandlerInstances.ContainsKey(messageType.Key))
                {
                    _messageHandlerInstances[messageType.Key] = new List<object>();
                }
                
                _messageHandlerInstances[messageType.Key].Add(handler);
            }
        }
    }
    
    public async Task Publish<T>(T message) where T : IMessage
    {
        var messageType = message.GetType();
        if (_messageHandlerInstances.TryGetValue(messageType, out var instance))
        {
            foreach (var casted in instance.Cast<IMessageHandler<T>?>())
            {
                await casted?.Handle(message)!;
            }
        }
    }
}
