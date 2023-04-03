using Data.DbContext;
using Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class HashesController : ControllerBase
{
    private readonly IHashGenerator _hashGenerator;
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly HashDbContext _hashDbContext;

    public HashesController(
        IHashGenerator hashGenerator,
        IMessageQueueClient messageQueueClient,
        HashDbContext hashDbContext
    )
    {
        _hashGenerator = hashGenerator;
        _messageQueueClient = messageQueueClient;
        _hashDbContext = hashDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateHashes()
    {
        foreach (var hash in _hashGenerator.GenerateHashes(40000).Chunk(100))
        {
            await _messageQueueClient.Submit(hash);
        }
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetHashes()
    {
        return Ok(
            await _hashDbContext.Hashes
                .GroupBy(h => h.Date)
                .Select(h => new { Date = h.Key, Count = h.Count() })
                .ToListAsync()
        );
    }
}
