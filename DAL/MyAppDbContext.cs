using Microsoft.EntityFrameworkCore;
using test_crud_with_jwt.Models;

namespace test_crud_with_jwt.DAL
{
    public class MyAppDbContext : DbContext
    {
        public MyAppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Books> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User-Hobby relationship
            modelBuilder.Entity<Hobby>()
                .HasOne(h => h.User)
                .WithMany(u => u.Hobbies)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for better performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
