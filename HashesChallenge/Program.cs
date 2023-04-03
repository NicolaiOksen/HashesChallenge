using Data.DbContext;
using Data.Interfaces;
using Data.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HashDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))
);

builder.Services.AddSingleton<IHashGenerator, HashGenerator>();
builder.Services.AddScoped<IMessageQueueClient, RabbitMqMessageClient>();

builder.Services.AddSingleton(
    sp =>
        new ConnectionFactory
        {
            Uri = new(builder.Configuration.GetConnectionString("RabbitMQ") ?? string.Empty)
        }.CreateConnection()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
