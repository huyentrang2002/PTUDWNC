using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        //Tìm 1 bài viết có tên định danh là slug
        //được đăng vào tháng, năm
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

        //Tìm một thẻ (Tag) theo tên định danh (slug)
        public Task<Tag> GetTagAsyn(string slug,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>();
            return tagQuery.Where(t => t.UrlSlug.Contains(slug))
                .FirstOrDefaultAsync(cancellationToken);           
        }

        
        // Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó
        public async Task<IList<TagItem>> GetListTagAsync(string tag,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>()
                //moi tag se thuoc ve nhieu bai post khac nhau
                //muon biet cac bai post thuoc ve 1 tag trong ef can anh xa cac bai post
                // bang cach su dung ham include va chi ra thuoc tinh post can anh xa
                .Include(t =>t.Posts)
                .Select(x => new TagItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    PostCount = x.Posts.Count(p => p.Published),
                });

            return await tagQuery.ToListAsync(cancellationToken);
        }

        // Xóa một thẻ theo mã cho trước
        public async Task DeleteTagByIDAcsyn(int? id,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = await _context.Set<Tag>().FindAsync(id);
            if (tagQuery != null)
            {
                Tag tagcontext = tagQuery;
                _context.Tags.Remove(tagcontext);

                await _context.SaveChangesAsync(cancellationToken);                
            }            
                
        }

        //Tìm một chuyên mục(Category) theo tên định danh(slug)
        public Task<Category> GetCategoriesBySlugAsync(string slug,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Category>();
            return tagQuery.Where(t => t.UrlSlug.Contains(slug))
                .FirstOrDefaultAsync(cancellationToken);
        }

    }
}
