using System.Text;
using Data.DbContext;
using Data.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Processor;

public class HashProcessor : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConnection _rabbitMqConnection;
    private readonly ILogger<HashProcessor> _logger;
    private readonly IModel _rabbitMqChannel;
    private readonly SemaphoreSlim _semaphore;

    public HashProcessor(
        IServiceProvider provider,
        IConnection rabbitMqConnection,
        ILogger<HashProcessor> logger
    )
    {
        _rabbitMqChannel = rabbitMqConnection.CreateModel();
        _rabbitMqChannel.QueueDeclare(
            queue: "hash-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        _provider = provider;
        _rabbitMqConnection = rabbitMqConnection;
        _logger = logger;
        _semaphore = new(4);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_rabbitMqChannel);

        consumer.Received += async (model, ea) =>
        {
            // Wait for a free slot in the semaphore
            await _semaphore.WaitAsync(stoppingToken);

            await using var scope = _provider.CreateAsyncScope();

            var hashDbContext = scope.ServiceProvider.GetRequiredService<HashDbContext>();

            await Task.Run(
                async () =>
                {
                    try
                    {
                        var t = ea.Body.ToArray();
                        var hashEntry = new Hash
                        {
                            Date = DateTime.UtcNow.Date,
                            Sha1 = Encoding.UTF8.GetString(ea.Body.ToArray())
                        };

                        await hashDbContext.Hashes.AddAsync(hashEntry, stoppingToken);

                        await hashDbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Hash added to database");
                    }
                    finally
                    {
                        _semaphore.Release(); // Release the slot in the semaphore
                    }
                },
                stoppingToken
            );
        };

        _rabbitMqChannel.BasicConsume(queue: "hash-queue", autoAck: true, consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
