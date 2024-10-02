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
    public int Count { get; set; } = 0;
    
    public TestHandler()
    {
        Console.WriteLine("TestHandler created.");
    }
    
    public async Task Handle(MOne message)
    {
        Count++;
        Console.WriteLine($"Received Message {message.Data} MOne in Test Handler. Count: {Count}");
    }

    public async Task Handle(MTwo message)
    {
        Count++;
        Console.WriteLine($"Received Message {message.Data} from MTwo in Test Handler. Count: {Count}");
    }

    public async Task Handle(MThree message)
    {
        Count++;
        Console.WriteLine($"Received Message {message.Data} from MThree in Test Handler. Count: {Count}");
    }
}

public class TestTwoHandler : IMessageHandler<MTwo>
{
    public int Count { get; set; } = 0;
    
    public TestTwoHandler()
    {
        Console.WriteLine("TestTwoHandler created.");
    }
    public async Task Handle(MTwo message)
    {
        Count++;
        Console.WriteLine($"Received Message {message.Data} from MTwo in TestTwo Handler. Count: {Count}");
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var bus = new Transport(AppDomain.CurrentDomain);
        
        await bus.Publish(new MTwo("HEY LOOK IT WORKS."));
        await bus.Publish(new MOne("HEY LOOK IT WORKS."));
        await bus.Publish(new MThree("HEY LOOK IT WORKS."));
        
        var t = "";
    }
}