using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Service.Extensions;

namespace TatBlog.Service.Blogs
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task<Post> GetPostAsyn(
            int year,
            int month,
            string slug,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postsQuery = _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Author);
            if (year > 0)
                postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
            if (month > 0)
                postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
            if(!string.IsNullOrEmpty(slug))
                postsQuery = postsQuery.Where(x => x.UrlSlug== slug);
            return await postsQuery.FirstOrDefaultAsync(cancellationToken);
        }
        
        public async Task<IList<Post>> GetPopularArticAsync(
            int numPosts,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(x =>x.Author)
                .Include(x => x.Category)
                .OrderByDescending(p => p.ViewCount)
                .Take(numPosts)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsPostSlugExistAsync(
            int postId,
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .AnyAsync(x => x.Id != postId && x.UrlSlug == slug,
                cancellationToken);
        }
        
        public async Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == postId)
                .ExecuteUpdateAsync(p=>
                p.SetProperty (x => x.ViewCount,x => x.ViewCount +1),
                cancellationToken);
        }

        public async Task<IList<Category>> GetCatergoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> catergories = _context.Set<Category>();
            if (showOnMenu)
            {
                catergories = catergories.Where(x => x.ShowOnMenu);
            }
            return await catergories
                .OrderBy(x=>x.Name)
                .Select(x=> new Category()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    ShowOnMenu = x.ShowOnMenu,
                    PostCount = x.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>()
                .Select(x => new TagItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,                   
                    PostCount = x.Posts.Count(p => p.Published),
                });

            return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);                
        }

        public async Task<List<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Category> categories = _context.Set<Category>();

            if (showOnMenu)
            {
                categories = categories.Where(x => x.ShowOnMenu);
            }
            return await categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description= x.Description,
                    ShowOnMenu = x.ShowOnMenu,
                    PostCount= x.Posts.Count(p => p.Published)

                })
                .ToListAsync(cancellationToken);

        }
    }
}
