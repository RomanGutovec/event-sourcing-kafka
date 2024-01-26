using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)

    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (expectedVersion == -1 && eventStream[^1].Version != expectedVersion)
            throw new ConcurrencyException();

        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException($"Post with id {aggregateId} was not found");

        var version = expectedVersion;
        foreach (var @event in events)
        {
            @event.Version = version++;
            var eventModel = new EventModel()
            {
                TimeStamp = DateTime.Now,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = @event.GetType().Name,
                EventData = @event
            };

            await _eventStoreRepository.SaveAsync(eventModel);

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _eventProducer.ProduceAsync(topic, @event);
        }
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException($"Post with id {aggregateId} was not found");

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }
}