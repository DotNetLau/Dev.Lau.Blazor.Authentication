using Azure.Messaging.ServiceBus;
using Dev.Lau.Blazor.Authentication.Models;

namespace Dev.Lau.Blazor.Authentication.HostedServices;

public class ServiceBusListener : IHostedService
{
    private readonly ServiceBusProcessor _processor;

    public ServiceBusListener(IServiceProvider ServiceProvider)
    {
        var scope = ServiceProvider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<ServiceBusClient>();

        _processor = client.CreateProcessor("teqit-playground-dev-count-increased", new ServiceBusProcessorOptions());
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        var deserializedBody = System.Text.Json.JsonSerializer.Deserialize<CounterDto>(body);
        Console.WriteLine($"Received: {deserializedBody}");

        // complete the message. message is deleted from the queue.
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}