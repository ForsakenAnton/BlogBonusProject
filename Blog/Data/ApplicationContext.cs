using Blog.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Blog.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        public DbSet<Post> Posts => Set<Post>(); // { get; set; } = default!; то же самое
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Comment> Comments => Set<Comment>();
        //public DbSet<User> Users { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Comment>()
        //    .HasOne(c => c.ParentComment)
        //    .WithMany(c => c.ChildComments)
        //    .HasForeignKey(c => c.ParentCommentId)
        //    .OnDelete(DeleteBehavior.Cascade);
        //}
    }
}
