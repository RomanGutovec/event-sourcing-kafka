using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IEventHandler _eventHandler;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IServiceProvider _serviceProvider;

    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler, IServiceScopeFactory serviceScopeFactory)
    {
        _eventHandler = eventHandler;
        _serviceScopeFactory = serviceScopeFactory;
        _config = config.Value;
    }

    public void Consume(string topic)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);
        while (true)
        {
            var consumerResult = consumer.Consume();
            if (consumerResult?.Message is null) continue;
            var options = new JsonSerializerOptions()
            {
                Converters = { new EventJsonConverter() }
            };

            var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);

            var handlerMethod = _eventHandler.GetType()
                .GetMethod(nameof(IEventHandler.HandleAsync), new Type[] { @event.GetType() });

            if (handlerMethod is null)
            {
                throw new NullReferenceException("Could not find event handler method.");
            }

            handlerMethod.Invoke(_eventHandler, new object[] { @event });
            consumer.Commit(consumerResult);
        }
    }
}