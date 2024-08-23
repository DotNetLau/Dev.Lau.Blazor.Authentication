using Azure.Messaging.ServiceBus;
using Dev.Lau.Blazor.Authentication.Models;

namespace Dev.Lau.Blazor.Authentication.Services;

public class ServiceBusService(ServiceBusClient _serviceBusClient)
{
    private const int numOfMessages = 3;

    public async Task Dispatch(int count)
    {
        try
        {
            var serializedMessage = System.Text.Json.JsonSerializer.Serialize(new CounterDto
            {
                Count = count
            });
            var message = new ServiceBusMessage(serializedMessage);

            await using var sender = _serviceBusClient.CreateSender("teqit-playground-dev-count-increased");

            await sender.SendMessageAsync(message);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}