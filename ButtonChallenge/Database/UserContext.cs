using Microsoft.EntityFrameworkCore;

namespace ButtonChallenge.Database
{
    /// <summary>
    /// This is essentially the session/interface with the database when using EF.
    /// </summary>
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);
            // Allows for optimistic concurrency. On commit, if the user was
            // modified (e.g. a transfer happens), an exception will occur.
            modelBuilder.Entity<User>()
                .Property(u => u.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Transfer>()
                .HasKey(t => t.TransferId);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
    }
}
