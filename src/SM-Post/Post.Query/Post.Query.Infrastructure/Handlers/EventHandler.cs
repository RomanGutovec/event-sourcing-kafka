using Microsoft.EntityFrameworkCore;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Infrastructure.DataPersistence.Interfaces;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostDbContext _dbContext;

    public EventHandler(IPostDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(PostCreatedEvent @event)
    {
        _dbContext.Posts.Add(new PostEntity()
            { PostId = @event.Id, Author = @event.Author, DatePosted = @event.DatePosted, Message = @event.Message });
        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleAsync(CommentAddedEvent @event)
    {
        await _dbContext.Comments.AddAsync(new CommentEntity()
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            UserName = @event.UserName,
            Edited = false
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleAsync(CommentRemovedEvent @event)
    {
        var comment = await _dbContext.Comments.FirstOrDefaultAsync(x => x.CommentId == @event.CommentId);

        if (comment is null) return;

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleAsync(CommentUpdatedEvent @event)
    {
        var comment = await _dbContext.Comments.FirstOrDefaultAsync(x => x.CommentId == @event.CommentId);

        if (comment is null) return;

        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleAsync(MessageUpdatedEvent @event)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.PostId == @event.Id);

        if (post is null) return;

        post.Message = @event.Message;
        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleAsync(PostLikedEvent @event)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.PostId == @event.Id);

        if (post is null) return;

        post.Likes++;
        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleAsync(PostRemovedEvent @event)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.PostId == @event.Id);

        if (post is null) return;

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
    }
}