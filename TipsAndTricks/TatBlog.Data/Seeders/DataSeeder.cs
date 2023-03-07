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

        var author = AddAuthors();
        var categories = AddCategories();
        var tags = AddTags();
        var posts = AddPosts(author, categories, tags);
    }

    private IList<Author> AddAuthors()
    {
        var authors = new List<Author>() {
                new() {
                    FullName = "Jason Mouth",
                    UrlSlug = "json-mouth",
                    Email = "json@gmail.com",
                    JoinedDate = new DateTime(2022, 10, 21)
                },
                new() {
                    FullName = "Jessica Wonder",
                    UrlSlug = "jessica-wonder",
                    Email = "jessica665@motip.com",
                    JoinedDate = new DateTime(2020, 4, 19)
                },
                new() {
                    FullName = "Leanne Graham",
                    UrlSlug = "leanne-graham",
                    Email = "leanne@gmail.com",
                    JoinedDate = new DateTime(2022, 12, 1)
                },
                new() {
                    FullName = "Ervin Howell",
                    UrlSlug = "ervin-howell",
                    Email = "ervin@gmail.com",
                    JoinedDate = new DateTime(2023, 1, 22)
                },
                new() {
                    FullName = "Clementine Bauch",
                    UrlSlug = "clementine-bauch",
                    Email = "clementine@gmail.com",
                    JoinedDate = new DateTime(2022, 11, 23)
                },
                new() {
                    FullName = "Patricia Lebsack",
                    UrlSlug = "patricia-lebsack",
                    Email = "patricia@gmail.com",
                    JoinedDate = new DateTime(2021, 7, 8)
                },
                new() {
                    FullName = "Chelsey Dietrich",
                    UrlSlug = "chelsey-dietrich",
                    Email = "chelsey@gmail.com",
                    JoinedDate = new DateTime(2022, 3, 14)
                }
            };

        _dbContext.Authors.AddRange(authors);
        _dbContext.SaveChanges();

        return authors;
    }
    private IList<Category> AddCategories()
    {
        var categories = new List<Category>() {
                new() {Name = ".NET Core", Description = ".NET Core", UrlSlug = "net-core"},
                new() {Name = "Architecture", Description = "Architecture", UrlSlug = "architecture"},
                new() {Name = "Messaging", Description = "Messaging", UrlSlug = "messaging"},
                new() {Name = "OOP", Description = "Object-Oriented Program", UrlSlug = "oop"},
                new() {Name = "Design Patterns", Description = "Design Patterns", UrlSlug = "design-patterns"},
                new() {Name = "React", Description = "React", UrlSlug = "react"},
                new() {Name = "Angular", Description = "Angular", UrlSlug = "angular"},
                new() {Name = "Vue.js", Description = "Vue.js", UrlSlug = "vue-js"},
                new() {Name = "Next.js", Description = "Next.js", UrlSlug = "next-js"},
                new() {Name = "Node.js", Description = "Node.js", UrlSlug = "node-js"},
                new() {Name = "Golang", Description = "Golang", UrlSlug = "golang"},
                new() {Name = "Three.js", Description = "Three.js", UrlSlug = "three-js"},
                new() {Name = "PHP", Description = "PHP", UrlSlug = "php"},
                new() {Name = "Laravel", Description = "Laravel", UrlSlug = "laravel"},
                new() {Name = "Svelte", Description = "Svelte", UrlSlug = "svelte"}
            };

        _dbContext.Categories.AddRange(categories);
        _dbContext.SaveChanges();

        return categories;
    }
    private IList<Tag> AddTags()
    {
        var tags = new List<Tag>() {
                new() {Name = "Google", Description = "Google", UrlSlug = "google"},
                new() {Name = "ASP.NET MVC", Description = "ASP.NET MVC", UrlSlug = "asp-net-mvc"},
                new() {Name = "Razor Page", Description = "Razor Page", UrlSlug = "razor-page"},
                new() {Name = "Blazor", Description = "Blazor", UrlSlug = "blazor"},
                new() {Name = "Deep Learning", Description = "Deep Learning", UrlSlug = "deep-learning"},
                new() {Name = "Neural Network", Description = "Neural Network", UrlSlug = "neural-network"},
                new() {Name = "Front-End Applications", Description = "Front-End Applications", UrlSlug = "font-end-application"},
                new() {Name = "Visual Studio", Description = "Visual Studio", UrlSlug = "visual-studio"},
                new() {Name = "SQL Server", Description = "SQL Server", UrlSlug = "sql-server"},
                new() {Name = "Git", Description = "Git", UrlSlug = "git"},
                new() {Name = "Entity Framework Core", Description = "EF Core", UrlSlug = "entity-framework-core"},
                new() {Name = ".NET Framework", Description = ".NET Framework", UrlSlug = "net-framework"},
                new() {Name = "ASP.NET Core", Description = "ASP.NET Core", UrlSlug = "aspnet-core"},
                new() {Name = "Postman", Description = "Postman", UrlSlug = "postman"},
                new() {Name = "ChatGPT", Description = "Chat GPT", UrlSlug = "chat-gpt"},
                new() {Name = "Data cleansing", Description = "Data cleaning", UrlSlug = "data-cleansing"},
                new() {Name = "Fetch API", Description = "Fetch API", UrlSlug = "fetch-api"},
                new() {Name = "Microsoft", Description = "Microsoft", UrlSlug = "microsoft"},
                new() {Name = "Microservices", Description = "Microservices", UrlSlug = "microservices"},
                new() {Name = "Web API Security", Description = "Web API Security", UrlSlug = "web-api-security"}
            };

        _dbContext.Tags.AddRange(tags);
        _dbContext.SaveChanges();

        return tags;
    }
    private IList<Post> AddPosts(IList<Author> authors, IList<Category> categories, IList<Tag> tags)
    {
        var posts = new List<Post>() {
                new() {
                    Title = "His mother had always taught him",
                    ShortDescription = "His mother had always taught him not to ever think of himself as better than others.",
                    Description = "His mother had always taught him not to ever think of himself as better than others. He'd tried to live by this motto. He never looked down on those who were less fortunate or who had less money than him. But the stupidity of the group of people he was talking to made him change his mind.",
                    Meta = "post-01",
                    UrlSlug = "his-mother-had-always-taught-him",
                    Published = true,
                    PostedDate = new DateTime(2023, 2, 22, 1, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 2,
                    Author = authors[0],
                    Category = categories[0],
                    Tags = new List<Tag>()
                    {
                        tags[0],
                        tags[1],
                        tags[2],
                    }
                },
                new() {
                    Title = "He was an expert but not in a discipline",
                    ShortDescription = "He was an expert but not in a discipline that anyone could fully appreciate.",
                    Description = "He was an expert but not in a discipline that anyone could fully appreciate. He knew how to hold the cone just right so that the soft server ice-cream fell into it at the precise angle to form a perfect cone each and every time. It had taken years to perfect and he could now do it without even putting any thought behind it.",
                    Meta = "post-02",
                    UrlSlug = "he-was-an-expert-but-not-in-a-discipline",
                    Published = true,
                    PostedDate = new DateTime(2022, 12, 1, 7, 6, 0),
                    ModifiedDate = null,
                    ViewCount = 2,
                    Author = authors[2],
                    Category = categories[4],
                    Tags = new List<Tag>()
                    {
                        tags[2],
                        tags[3],
                        tags[4],
                    }
                },
                new() {
                    Title = "Dave watched as the forest burned up on the hill",
                    ShortDescription = "Dave watched as the forest burned up on the hill.",
                    Description = "Dave watched as the forest burned up on the hill, only a few miles from her house. The car had been hastily packed and Marta was inside trying to round up the last of the pets. Dave went through his mental list of the most important papers and documents that they couldn't leave behind. He scolded himself for not having prepared these better in advance and hoped that he had remembered everything that was needed. He continued to wait for Marta to appear with the pets, but she still was nowhere to be seen.",
                    Meta = "post-03",
                    UrlSlug = "dave-watched-as-the-forest-burned-up-on-the-hill",
                    Published = true,
                    PostedDate = new DateTime(2023, 1, 26, 6, 1, 0),
                    ModifiedDate = null,
                    ViewCount = 5,
                    Author = authors[3],
                    Category = categories[5],
                    Tags = new List<Tag>()
                    {
                        tags[3],
                        tags[4],
                        tags[5],
                    }
                },
                new() {
                    Title = "All he wanted was a candy bar.",
                    ShortDescription = "All he wanted was a candy bar.",
                    Description = "All he wanted was a candy bar. It didn't seem like a difficult request to comprehend, but the clerk remained frozen and didn't seem to want to honor the request. It might have had something to do with the gun pointed at his face.",
                    Meta = "post-04",
                    UrlSlug = "all-he-wanted-was-a-candy-bar",
                    Published = true,
                    PostedDate = new DateTime(2022, 11, 1, 6, 32, 0),
                    ModifiedDate = null,
                    ViewCount = 1,
                    Author = authors[3],
                    Category = categories[6],
                    Tags = new List<Tag>()
                    {
                        tags[4],
                        tags[5],
                    }
                },
                new() {
                    Title = "Hopes and dreams were dashed that day",
                    ShortDescription = "Hopes and dreams were dashed that day.",
                    Description = "Hopes and dreams were dashed that day. It should have been expected, but it still came as a shock. The warning signs had been ignored in favor of the possibility, however remote, that it could actually happen. That possibility had grown from hope to an undeniable belief it must be destiny. That was until it wasn't and the hopes and dreams came crashing down.",
                    Meta = "post-05",
                    UrlSlug = "hopes-and-dreams-were-dashed-that-day",
                    Published = true,
                    PostedDate = new DateTime(2022, 10, 7, 7, 53, 0),
                    ModifiedDate = null,
                    ViewCount = 2,
                    Author = authors[1],
                    Category = categories[7],
                    Tags = new List<Tag>()
                    {
                        tags[7],
                        tags[1],
                        tags[2],
                    }
                },
                new() {
                    Title = "Dave wasn't exactly sure how he had ended up in this predicament",
                    ShortDescription = "Dave wasn't exactly sure how he had ended up in this predicament.",
                    Description = "Dave wasn't exactly sure how he had ended up in this predicament. He ran through all the events that had lead to this current situation and it still didn't make sense. He wanted to spend some time to try and make sense of it all, but he had higher priorities at the moment. The first was how to get out of his current situation of being naked in a tree with snow falling all around and no way for him to get down.",
                    Meta = "post-06",
                    UrlSlug = "dave-wasnt-exactly-sure-how-he-had-ended-up-in-this-predicament",
                    Published = true,
                    PostedDate = new DateTime(2022, 4, 30, 8, 0, 0),
                    ModifiedDate = null,
                    ViewCount = 3,
                    Author = authors[4],
                    Category = categories[4],
                    Tags = new List<Tag>()
                    {
                        tags[5],
                        tags[1],
                        tags[2],
                    }
                },
                new() {
                    Title = "This is important to remember",
                    ShortDescription = "This is important to remember.",
                    Description = "This is important to remember. Love isn't like pie. You don't need to divide it among all your friends and loved ones. No matter how much love you give, you can always give more. It doesn't run out, so don't try to hold back giving it as if it may one day run out. Give it freely and as much as you want.",
                    Meta = "post-07",
                    UrlSlug = "this-is-important-to-remember",
                    Published = true,
                    PostedDate = new DateTime(2022, 12, 1, 3, 8, 0),
                    ModifiedDate = null,
                    ViewCount = 8,
                    Author = authors[4],
                    Category = categories[5],
                    Tags = new List<Tag>()
                    {
                        tags[0],
                        tags[1],
                        tags[2],
                    }
                },
                new() {
                    Title = "One can cook on and with an open fire",
                    ShortDescription = "One can cook on and with an open fire.",
                    Description = "One can cook on and with an open fire. These are some of the ways to cook with fire outside. Cooking meat using a spit is a great way to evenly cook meat. In order to keep meat from burning, it's best to slowly rotate it.",
                    Meta = "post-08",
                    UrlSlug = "one-can-cook-on-and-with-an-open-fire",
                    Published = true,
                    PostedDate = new DateTime(2023, 2, 22, 2, 12, 0),
                    ModifiedDate = null,
                    ViewCount = 9,
                    Author = authors[1],
                    Category = categories[9],
                    Tags = new List<Tag>()
                    {
                        tags[9],
                        tags[5],
                        tags[15],
                    }
                },
                new() {
                    Title = "There are different types of secrets",
                    ShortDescription = "There are different types of secrets.",
                    Description = "There are different types of secrets. She had held onto plenty of them during her life, but this one was different. She found herself holding onto the worst type. It was the type of secret that could gnaw away at your insides if you didn't tell someone about it, but it could end up getting you killed if you did.",
                    Meta = "post-09",
                    UrlSlug = "there-are-different-types-of-secrets",
                    Published = true,
                    PostedDate = new DateTime(2022, 11, 8, 12, 2, 0),
                    ModifiedDate = null,
                    ViewCount = 2,
                    Author = authors[3],
                    Category = categories[6],
                    Tags = new List<Tag>()
                    {
                        tags[10],
                        tags[13],
                        tags[14],
                    }
                },
                new() {
                    Title = "They rushed out the door",
                    ShortDescription = "They rushed out the door, grabbing anything and everything they could think of they might need.",
                    Description = "They rushed out the door, grabbing anything and everything they could think of they might need. There was no time to double-check to make sure they weren't leaving something important behind. Everything was thrown into the car and they sped off. Thirty minutes later they were safe and that was when it dawned on them that they had forgotten the most important thing of all.",
                    Meta = "post-10",
                    UrlSlug = "they-rushed-out-the-door",
                    Published = true,
                    PostedDate = new DateTime(2022, 6, 7, 8, 9, 0),
                    ModifiedDate = null,
                    ViewCount = 4,
                    Author = authors[2],
                    Category = categories[6],
                    Tags = new List<Tag>()
                    {
                        tags[10],
                        tags[15],
                        tags[14],
                    }
                }
            };

        _dbContext.Posts.AddRange(posts);
        _dbContext.SaveChanges();

        return posts;
    }
}