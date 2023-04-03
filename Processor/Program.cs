using Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Processor;
using RabbitMQ.Client;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<HashDbContext>(
            (sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.UseSqlServer(config.GetConnectionString("SqlServer"));
            }
        );

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new ConnectionFactory
            {
                Uri = new(config.GetConnectionString("RabbitMQ") ?? string.Empty)
            }.CreateConnection();
        });

        services.AddHostedService<HashProcessor>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HashDbContext>();
    dbContext.Database.Migrate();
}

await host.RunAsync();
