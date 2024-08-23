namespace Dev.Lau.Blazor.Authentication.Models;

public class CounterDto
{
    public int Count { get; set; }
    public Guid MessageId { get; set; } = Guid.NewGuid();
}