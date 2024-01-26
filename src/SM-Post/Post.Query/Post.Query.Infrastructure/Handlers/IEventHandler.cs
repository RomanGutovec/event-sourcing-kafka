using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers;

public interface IEventHandler
{
    Task HandleAsync(PostCreatedEvent @event);
    Task HandleAsync(CommentAddedEvent @event);
    Task HandleAsync(CommentRemovedEvent @event);
    Task HandleAsync(CommentUpdatedEvent @event);
    Task HandleAsync(MessageUpdatedEvent @event);
    Task HandleAsync(PostLikedEvent @event);
    Task HandleAsync(PostRemovedEvent @event);
}