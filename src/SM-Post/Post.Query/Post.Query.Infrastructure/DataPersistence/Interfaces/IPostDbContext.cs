using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.DataPersistence.Interfaces;

public interface IPostDbContext
{
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}