using Data.Models;

namespace Data.Interfaces;

public interface IHashGenerator
{
    IEnumerable<ReadOnlyMemory<byte>> GenerateHashes(int count);
}
