using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Entities;
using TatBlog.Data.Mappings;
namespace TatBlog.Data.Contexts;
public class BlogDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Post> Posts{ get; set; }
    public DbSet<Tag> Tags { get; set; }

    public DbSet<Subscriber> Subscribers { get; set; }


    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }

    public BlogDbContext()
    {
    }

    //protected override void OnConfiguring(
    //    DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=TatBlog;
    //                                Trusted_Connection=false;MultipleActiveResultSets=true;
    //                                TrustServerCertificate=True;User ID=sa;pwd=123");
    //}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CategoryMap).Assembly);
    }
}
