namespace Redis.Bus.Interfaces;

public interface IMessage { }

public interface IMessageHandler<in T> where T : IMessage
{
    Task Handle(T message);
}