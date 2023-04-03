using Data.Interfaces;
using RabbitMQ.Client;

namespace Data.Services;

public class RabbitMqMessageClient : IMessageQueueClient
{
    private readonly IConnection _rabbitMqConnection;
    private IModel _rabbitMqChannel;

    public RabbitMqMessageClient(IConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;

        _rabbitMqChannel = rabbitMqConnection.CreateModel();
        _rabbitMqChannel.QueueDeclare(
            "hash-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public async Task Submit(IEnumerable<ReadOnlyMemory<byte>> messages)
    {
        var batchPublish = _rabbitMqChannel.CreateBasicPublishBatch();

        foreach (var message in messages)
        {
            batchPublish.Add(
                exchange: "",
                routingKey: "hash-queue",
                mandatory: true,
                properties: null,
                body: message
            );
        }

        batchPublish.Publish();
    }
}
