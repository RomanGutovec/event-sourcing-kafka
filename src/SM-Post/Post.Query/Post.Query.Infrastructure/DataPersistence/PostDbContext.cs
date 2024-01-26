using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Infrastructure.DataPersistence.Interfaces;

namespace Post.Query.Infrastructure.DataPersistence;

public class PostDbContext : DbContext, IPostDbContext
{
    public PostDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
}