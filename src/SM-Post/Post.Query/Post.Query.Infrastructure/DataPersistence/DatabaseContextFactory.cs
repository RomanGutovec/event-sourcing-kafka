using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataPersistence
{
    public class DatabaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;

        public DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public PostDbContext CreateDbContext()
        {
            DbContextOptionsBuilder<PostDbContext> optionsBuilder = new();
            _configureDbContext(optionsBuilder);

            return new PostDbContext(optionsBuilder.Options);
        }
    }
}