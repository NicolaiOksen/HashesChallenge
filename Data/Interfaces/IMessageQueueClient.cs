namespace Data.Interfaces;

public interface IMessageQueueClient
{
    Task Submit(IEnumerable<ReadOnlyMemory<byte>> messages);
}
