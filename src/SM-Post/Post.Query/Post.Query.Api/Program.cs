using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Api.QueryHandlers;
using Post.Query.Domain.Entities;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataPersistence;
using Post.Query.Infrastructure.DataPersistence.Interfaces;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Handlers;

var builder = WebApplication.CreateBuilder(args);

Action<DbContextOptionsBuilder> configureDbContext = o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SocialMediaConnectionString"));
builder.Services.AddDbContext<PostDbContext>(configureDbContext);
builder.Services.AddSingleton(new DatabaseContextFactory(configureDbContext));


builder.Services.AddScoped<IPostDbContext, PostDbContext>();

builder.Services.AddTransient<IQueryHandler, QueryHandler>();

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddTransient<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.AddTransient<IEventConsumer, EventConsumer>();

var dispatcher = new QueryDispatcher();
builder.Services.AddSingleton<IQueryDispatcher<PostEntity>>(dispatcher);

builder.Services.AddControllers();
builder.Services.AddHostedService<ConsumerHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//todo move to extension methods
using var scope = app.Services.CreateScope();
var dataContext = scope.ServiceProvider.GetRequiredService<PostDbContext>();
dataContext.Database.EnsureCreated();

//register handlers
using var serviceScope = app.Services.CreateScope();
var commandHandler = serviceScope.ServiceProvider.GetRequiredService<IQueryHandler>();

dispatcher.RegisterHandler<FindAllPostsQuery>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByAuthorQuery>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithLikesQuery>(commandHandler.HandleAsync);

app.MapControllers();
app.Run();