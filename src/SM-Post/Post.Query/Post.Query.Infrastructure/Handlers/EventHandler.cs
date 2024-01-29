using Microsoft.EntityFrameworkCore;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Infrastructure.DataPersistence;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly DatabaseContextFactory _contextFactory;

    public EventHandler(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task HandleAsync(PostCreatedEvent @event)
    {
        await using var context = _contextFactory.CreateDbContext();
        context.Posts.Add(new PostEntity()
            { PostId = @event.Id, Author = @event.Author, DatePosted = @event.DatePosted, Message = @event.Message });
        await context.SaveChangesAsync();
    }

    public async Task HandleAsync(CommentAddedEvent @event)
    {
        await using var context = _contextFactory.CreateDbContext();
        await context.Comments.AddAsync(new CommentEntity()
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            UserName = @event.UserName,
            Edited = false
        });
        await context.SaveChangesAsync();
    }

    public async Task HandleAsync(CommentRemovedEvent @event)
    {
        await using var context = _contextFactory.CreateDbContext();
        var comment = await context.Comments.FirstOrDefaultAsync(x => x.CommentId == @event.CommentId);

        if (comment is null) return;

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task HandleAsync(CommentUpdatedEvent @event)
    {
        await using var context = _contextFactory.CreateDbContext();
        var comment = await context.Comments.FirstOrDefaultAsync(x => x.CommentId == @event.CommentId);

        if (comment is null) return;

        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await context.SaveChangesAsync();
    }

    public async Task HandleAsync(MessageUpdatedEvent @event)
    {
        await using var context = _contextFactory.CreateDbContext();
        var post = await context.Posts.FirstOrDefaultAsync(x => x.PostId == @event.Id);

        if (post is null) return;

        post.Message = @event.Message;
        await context.SaveChangesAsync();
    }

    public async Task HandleAsync(PostLikedEvent @event)
    {
        await using var context = _contextFactory.CreateDbContext();
        var post = await context.Posts.FirstOrDefaultAsync(x => x.PostId == @event.Id);

        if (post is null) return;

        post.Likes++;
        await context.SaveChangesAsync();
    }

    public async Task HandleAsync(PostRemovedEvent @event)
    {
        await using var context = _contextFactory.CreateDbContext();
        var post = await context.Posts.FirstOrDefaultAsync(x => x.PostId == @event.Id);

        if (post is null) return;

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }
}