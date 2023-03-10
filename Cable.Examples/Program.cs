using Cable.Core;
using Cable.Interfaces;

namespace Redis.Bus.Examples;

public record MOne(string Data) : IMessage;

public record MTwo(string Data) : IMessage;

public record MThree(string Data) : IMessage;

public class TestHandler : IMessageHandler<MOne>, 
    IMessageHandler<MTwo>, 
    IMessageHandler<MThree>
{
    public async Task Handle(MOne message)
    {
        Console.WriteLine($"Received Message {message.Data} MOne");
    }

    public async Task Handle(MTwo message)
    {
        Console.WriteLine($"Received Message {message.Data} from MTwo");
    }

    public async Task Handle(MThree message)
    {
        Console.WriteLine($"Received Message {message.Data} from MThree");
    }
}

public class TestTwoHandler : IMessageHandler<MTwo>
{
    public async Task Handle(MTwo message)
    {
        Console.WriteLine($"Received Message {message.Data} from MTwo in TestTwo");
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var bus = new Transport(AppDomain.CurrentDomain);
        
        await bus.Publish(new MTwo("HEY LOOK IT WORKS."));
        
        var t = "";
    }
}