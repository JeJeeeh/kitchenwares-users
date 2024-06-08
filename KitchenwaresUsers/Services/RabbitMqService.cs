using System.Text;
using System.Text.Json;
using KitchenwaresUsers.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KitchenwaresUsers.Services;

public interface IRabbitMqService
{
    void StartConsuming();
    void SendMessage<T>(T message);
}

public class RabbitMqService(IUserService userService) : IRabbitMqService
{
    public void StartConsuming()
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ__HOST") ?? "localhost",
            Port = 5672,
            UserName = Environment.GetEnvironmentVariable("RABBITMQ__USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ__PASS") ?? "guest"
        };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "auth", 
            exclusive: false,
            durable: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var newUser = JsonSerializer.Deserialize<AuthUserRequest>(message);
            await userService.Create(newUser!);
            
        };
        
        channel.BasicConsume(queue: "auth",
            autoAck: true,
            consumer: consumer);
    }

    public void SendMessage<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ__HOST") ?? "localhost",
            Port = 5672,
            UserName = Environment.GetEnvironmentVariable("RABBITMQ__USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ__PASS") ?? "guest"
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("users", exclusive: false);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        
        channel.BasicPublish(exchange: "", routingKey: "users", body: body);
    }
}