using System.Security.Cryptography;
using System.Text;
using Data.Interfaces;
using Data.Models;

namespace Data.Services;

public class HashGenerator : IHashGenerator
{
    public IEnumerable<ReadOnlyMemory<byte>> GenerateHashes(int count)
    {
        using var sha1 = new SHA1Managed();

        for (var i = 0; i < count; i++)
        {
            yield return new(sha1.ComputeHash(RandomNumberGenerator.GetBytes(32)));
        }
    }
}
