using Confluent.Kafka;
using CQRS.Core.Consumers;
using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataPersistence;
using Post.Query.Infrastructure.DataPersistence.Interfaces;
using Post.Query.Infrastructure.Handlers;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<PostDbContext>(o => o
    .UseLazyLoadingProxies()

    .UseSqlServer(builder.Configuration.GetConnectionString("SocialMediaConnectionString")));

builder.Services.AddScoped<IPostDbContext>(provider => provider.GetRequiredService<PostDbContext>());


builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

builder.Services.AddHostedService<ConsumerHostedService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dataContext =scope.ServiceProvider.GetRequiredService<PostDbContext>();
dataContext.Database.EnsureCreated();
app.MapGet("/", () => "Hello World!");

app.Run();