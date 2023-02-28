using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeders;

public class DataSeeder : IDataSeeder
{
    private readonly BlogDbContext _dbContext;
    public DataSeeder(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void Initialize()
    {
        _dbContext.Database.EnsureCreated();

        if (_dbContext.Posts.Any()) return;

        var authors = AddAuthor();
        var categories = AddCategories();
        var tags = AddTags();
        var posts = AddPosts(authors, categories, tags);
    }

    private IList<Author> AddAuthor()
    {
        var authors = new List<Author>()
        {
            new()
            {
                FullName = "Jason Mouth",
                UrlSlug = "jason-mouth",
                Email = "json@gmail.com",
                JoinedDate = new DateTime(2022, 10, 21)
             },

            new()
            {
                FullName = "Jason Mouth1",
                UrlSlug = "jason1-mouth",
                Email = "json1@gmail.com",
                JoinedDate = new DateTime(2022, 4, 19)
            },
            new()
            {
                FullName = "Jason Mouth2",
                UrlSlug = "jason2-mouth",
                Email = "json2@gmail.com",
                JoinedDate = new DateTime(2022, 7, 5)
            },
            new()
            {
                FullName = "Jason Mouth3",
                UrlSlug = "jason3-mouth",
                Email = "json3@gmail.com",
                JoinedDate = new DateTime(2022, 11, 6)
            }
        };
        _dbContext.Authors.AddRange(authors);
        _dbContext.SaveChanges();

        return authors;
    }
    private IList<Category> AddCategories() 
    {
        var categories = new List<Category>()
        {
            new() { Name =".NET Core", Description ="123", UrlSlug ="www"},
            new() { Name =".NET Core", Description ="123", UrlSlug ="www"},
            new() { Name =".NET Core", Description ="123", UrlSlug ="www"},
            new() { Name =".NET Core", Description ="123", UrlSlug ="www"}
        };
        _dbContext.AddRange(categories);
        _dbContext.SaveChanges();

        return categories;
    }

    private IList<Tag> AddTags() 
    {
        var tags = new List<Tag>()
        {
            new () { Name ="Google", Description="123", UrlSlug ="weww"},
            new () { Name ="Mongo DB", Description="124", UrlSlug ="wdww"},
            new () { Name ="Taiwind CSS", Description="125", UrlSlug ="wcww"},
            new () { Name ="Razor Page", Description="126", UrlSlug ="wbww"},
            new () { Name ="Google", Description="127", UrlSlug ="waww"}
        };

        _dbContext.Tags.AddRange(tags);        
        _dbContext.SaveChanges();

        return tags;
    }
    private IList<Post> AddPosts(
        IList<Author> authors,
        IList<Category>catergories,
        IList<Tag> tags) 
    {
        var posts = new List<Post>()
        {
            new()
            {
                Title ="ASP.NET Core",
                ShortDescription ="1234",
                Description ="hihi",
                Meta = "hello",
                UrlSlug ="123",
                Published = true,
                PostedDate = new DateTime(2021,9,30,10,20,0),
                ModifiedDate = null,
                ViewCount = 10,
                Author = authors[0],
                Category = catergories[0],
                Tags = new List<Tag>()
                {
                    tags[0]
                }
            }
        };
        _dbContext.AddRange(posts);
        _dbContext.SaveChanges();

        return posts;
    }
}
