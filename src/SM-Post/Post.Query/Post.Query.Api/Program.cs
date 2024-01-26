using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.DataPersistence;
using Post.Query.Infrastructure.DataPersistence.Interfaces;
using Post.Query.Infrastructure.Handlers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddDbContext<PostDbContext>(o => o
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("SocialMediaConnectionString")));

builder.Services.AddScoped<IPostDbContext>(provider => provider.GetRequiredService<PostDbContext>());

builder.Services.AddTransient<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();

app.MapGet("/", () => "Hello World!");

app.Run();