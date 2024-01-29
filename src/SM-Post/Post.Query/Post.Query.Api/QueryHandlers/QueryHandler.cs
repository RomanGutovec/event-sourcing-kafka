using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Exceptions;
using Post.Query.Infrastructure.DataPersistence.Interfaces;

namespace Post.Query.Api.QueryHandlers;

public class QueryHandler : IQueryHandler
{
    private readonly IPostDbContext _context;

    public QueryHandler(IPostDbContext context)
    {
        _context = context;
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
    {
        return await _context.Posts.ToListAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == query.Id);
        if (post is null)
        {
            throw new EntityNotFoundException("Post was not found.");
        }

        return new List<PostEntity> { post };
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
    {
        return await _context.Posts.Where(x => x.Author == query.Author).ToListAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
    {
        return await _context.Posts.Where(x => x.Comments.Any()).ToListAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
    {
        return await _context.Posts.Where(x => x.Likes == query.NumberOfLikes).ToListAsync();
    }
}