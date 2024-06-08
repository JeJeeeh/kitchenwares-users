namespace KitchenwaresUsers.Services;

public class RabbitMqHostedService(IRabbitMqService rabbitMqService) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        rabbitMqService.StartConsuming();
        return Task.CompletedTask;
    }
}