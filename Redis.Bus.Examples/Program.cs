using Redis.Bus.Core;
using Redis.Bus.Interfaces;

public record MOne(string Data) : IMessage;

public record MTwo(string Data) : IMessage;

public record MThree(string Data) : IMessage;

public class TestHandler : IMessageHandler<MOne>, 
    IMessageHandler<MTwo>, 
    IMessageHandler<MThree>
{
    public async Task Handle(MOne message)
    {
        Console.WriteLine($"Received Message {message.Data} from Penis");
    }

    public async Task Handle(MTwo message)
    {
        Console.WriteLine($"Received Message {message.Data} from Penis2");
    }

    public async Task Handle(MThree message)
    {
        Console.WriteLine($"Received Message {message.Data} from Penis3");
    }
}

public class TestTwoHandler : IMessageHandler<MTwo>
{
    public async Task Handle(MTwo message)
    {
        Console.WriteLine($"Received Message {message.Data} from Penis2 in TestTwo");
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
