using Microsoft.EntityFrameworkCore;

namespace FileHost.Data
{
    public class FileHostDbContext : DbContext
    {
        public FileHostDbContext(DbContextOptions<FileHostDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FileRecord> Files { get; set; }
    }
}