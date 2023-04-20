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
            if (!string.IsNullOrEmpty(slug))
                postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
            return await postsQuery.FirstOrDefaultAsync(cancellationToken);
        }

        // Tìm Top N bài viết phổ được nhiều người xem nhất
        public async Task<IList<Post>> GetPopularArticAsync(
            int numPosts,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(x => x.Author)
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

        // Tăng số lượt xem của một bài viết
        public async Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == postId)
                .ExecuteUpdateAsync(p =>
                p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1),
                cancellationToken);
        }

        public async Task<IList<Category>> GetCategoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> Categories = _context.Set<Category>();
            if (showOnMenu)
            {
                Categories = Categories.Where(x => x.ShowOnMenu);
            }
            return await Categories
                .OrderBy(x => x.Name)
                .Select(x => new Category()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    PostCount = x.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }

        //chuyen published thanh unpublished
        public async Task<bool> TogglePublishedFlagAsync(
        int postId, CancellationToken cancellationToken = default)
        {
            var post = await _context.Set<Post>().FindAsync(postId);

            if (post is null) return false;

            post.Published = !post.Published;
            await _context.SaveChangesAsync(cancellationToken);

            return post.Published;
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

        public async Task<List<CategoryItem>> GetCategoryItemsAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
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
                    Description = x.Description,
                    ShowOnMenu = x.ShowOnMenu,
                    PostCount = x.Posts.Count(p => p.Published)

                })
                .ToListAsync(cancellationToken);

        }

        public async Task<Post> CreateOrUpdatePostAsync(
        Post post, IEnumerable<string> tags,
        CancellationToken cancellationToken = default)
        {
            if (post.Id > 0)
            {
                await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
            }
            else
            {
                post.Tags = new List<Tag>();
            }

            var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new
                {
                    Name = x,
                    Slug = x.GenerateSlug()
                })
                .GroupBy(x => x.Slug)
                .ToDictionary(g => g.Key, g => g.First().Name);


            foreach (var kv in validTags)
            {
                if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

                var tag = await GetTagAsync(kv.Key, cancellationToken) ?? new Tag()
                {
                    Name = kv.Value,
                    Description = kv.Value,
                    UrlSlug = kv.Key
                };

                post.Tags.Add(tag);
            }

            post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

            if (post.Id > 0)
                _context.Update(post);
            else
                _context.Add(post);

            await _context.SaveChangesAsync(cancellationToken);

            return post;
        }

        public async Task<int> CountPostsAsync(
        PostQuery condition, CancellationToken cancellationToken = default)
        {
            return await FilterPosts(condition).CountAsync(cancellationToken: cancellationToken);
        }

        public async Task<IList<PostInMonthItem>> CountMonthlyPostsAsync(
            int numMonths, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .GroupBy(x => new { x.PostedDate.Year, x.PostedDate.Month })
                .Select(g => new PostInMonthItem()
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(x => x.Published)
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category> CreateOrUpdateCategoryAsync(
        Category category, CancellationToken cancellationToken = default)
        {
            if (category.Id > 0)
            {
                _context.Set<Category>().Update(category);
            }
            else
            {
                _context.Set<Category>().Add(category);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return category;
        }

        //1a. Tìm một thẻ (Tag) theo tên định danh (slug)
        public Task<Tag> GetTagAsync(string slug,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>();
            return tagQuery.Where(t => t.UrlSlug.Contains(slug))
                .FirstOrDefaultAsync(cancellationToken);
        }

        //1b.Tạo lớp DTO có tên là TagItem

        //1c. Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó
        public async Task<IList<TagItem>> GetListTagAsync(string tag,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>()
                //moi tag se thuoc ve nhieu bai post khac nhau
                //muon biet cac bai post thuoc ve 1 tag trong ef can anh xa cac bai post
                // bang cach su dung ham include va chi ra thuoc tinh post can anh xa
                .Include(t => t.Posts)
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

        //1d. Xóa một thẻ theo mã cho trước
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
        #region CODE MẪU AUTHOR
        public async Task<Author> GetAuthorAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
        }

        //public async Task<Author> GetAuthorByIdAsync(int authorId)
        //{
        //    return await _context.Set<Author>().FindAsync(authorId);
        //}

        //public async Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _context.Set<Author>()
        //        .OrderBy(a => a.FullName)
        //        .Select(a => new AuthorItem()
        //        {
        //            Id = a.Id,
        //            FullName = a.FullName,
        //            Email = a.ToString(),
        //            JoinedDate = a.JoinedDate,
        //            ImageUrl = a.ImageUrl,
        //            UrlSlug = a.UrlSlug,
        //            Notes = a.Notes,
        //            PostCount = a.Posts.Count(p => p.Published)
        //        })
        //        .ToListAsync(cancellationToken);
        //}

        public async Task<IList<Post>> GetPostsAsync(
        PostQuery condition,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            return await FilterPosts(condition)
                .OrderByDescending(x => x.PostedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<IPagedList<Post>> GetPagedPostsAsync(
        PostQuery condition,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        {
            return await FilterPosts(condition).ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Post.PostedDate), "DESC",
                cancellationToken);
        }

        public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
            PostQuery condition,
            IPagingParams pagingParams,
            Func<IQueryable<Post>, IQueryable<T>> mapper)
        {
            var posts = FilterPosts(condition);
            var projectedPosts = mapper(posts);

            return await projectedPosts.ToPagedListAsync(pagingParams);
        }

        #endregion

        //1e. Tìm một chuyên mục(Category) theo tên định danh(slug)
        public Task<Category> GetCategoriesBySlugAsync(string slug,
            CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Category>();
            return tagQuery.Where(t => t.UrlSlug.Contains(slug))
                .FirstOrDefaultAsync(cancellationToken);
        }
        

        //1f. Tìm một chuyên mục theo mã số cho trước.
        public async Task<Category> GetCategoryByIdAsync
            (int id, CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Category>().FindAsync(id);
            return await _context.Categories.FindAsync(cancellationToken);
        }

        //1g. Thêm hoặc cập nhật một chuyên mục/chủ đề.
        public async Task AddOrUpdateCategoryAsync(Category cate,
            CancellationToken cancellationToken = default)
        {
            if (cate?.Id == null || _context.Categories == null)
            {
                await _context.Categories.AddAsync(cate, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return;

                var cat = await _context.Categories.FirstOrDefaultAsync(m => m.Id == cate.Id);
                cat.Name = cate.Name;
                cat.Description = cate.Description;
                cat.UrlSlug = cate.UrlSlug;
                cat.ShowOnMenu = cate.ShowOnMenu;

                await _context.SaveChangesAsync(cancellationToken);
            }

        }
        // 1h. Xóa một chuyên mục theo mã số cho trước.
        public async Task DeleteCategoryByIDAcsyn(int? id,
            CancellationToken cancellationToken = default)
        {
            var cateQuery = await _context.Set<Category>().FindAsync(id);
            if (cateQuery != null)
            {
                Category c = cateQuery;
                _context.Categories.Remove(c);

                await _context.SaveChangesAsync(cancellationToken);
            }

        }

        //1i. Kiểm tra tên định danh (slug) của một chuyên mục đã tồn tại hay chưa.
        public async Task<bool> CheckCategorySlugExisted(string slug)
        {
            var cateslug = await _context.Set<Category>()
                .AnyAsync(c => c.UrlSlug.Equals(slug));
            if (cateslug != null)
            {
                return true;
            }
            return false;
        }
        //1j. Lấy và phân trang danh sách chuyên mục, kết quả trả về kiểu IPagedList<CategoryItem>.
        public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
            IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Category>()
                                      .Select(x => new CategoryItem()
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          UrlSlug = x.UrlSlug,
                                          Description = x.Description,
                                          PostCount = x.Posts.Count(p => p.Published)
                                      });

            return await query.ToPagedListAsync(pagingParams, cancellationToken);
        }

        //1k. Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào. Kết quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số bài viết.

        //1l. Tìm một bài viết theo mã số.
        public async Task<Post> GetPostByIdAsync(int id,
            CancellationToken cancellationToken = default)
        {
            var Postid = await _context.Set<Post>().FindAsync(id);
            return Postid;
        }

        //public async Task<Post> GetPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default)
        //{
        //    IQueryable<Post> postQuery = _context.Set<Post>()
        //                             .Include(p => p.Category)
        //                             .Include(p => p.Author)
        //                             .Include(p => p.Tags);

        //    if (published)
        //    {
        //        postQuery = postQuery.Where(x => x.Published);
        //    }

        //    return await postQuery.Where(p => p.Id.Equals(id))
        //                          .FirstOrDefaultAsync(cancellationToken);
        //}

        public async Task<Post> GetPostByIdAsync(
        int postId, bool includeDetails = false,
        CancellationToken cancellationToken = default)
        {
            if (!includeDetails)
            {
                return await _context.Set<Post>().FindAsync(postId);
            }

            return await _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
        }

        //1m. Thêm hay cập nhật một bài viết
        public async Task AddOrUpdatePostAsync(Post post,
            CancellationToken cancellationToken = default)
        {
            var postget = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == post.Id);

            if (postget == null)
            {
                Console.WriteLine("Không có post nào để sửa");
                return;
            }

            postget.Title = post.Title;
            postget.Description = post.Description;
            postget.UrlSlug = post.UrlSlug;
            postget.Published = post.Published;

            _context.Attach(postget).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        //1n. Chuyển đổi trạng thái Published của bài viết
        public async Task ChangePostStatusAsync(int id,
            CancellationToken cancellationToken = default)
        {
            var post = await _context.Posts.FindAsync(id);

            post.Published = !post.Published;

            _context.Attach(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        //1o. Lấy ngẫu nhiên N bài viết. N là tham số đầu vào.
        public async Task<IList<Post>> GetRandomPostAsync(int n, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
               .OrderBy(x => Guid.NewGuid())
               .Take(n)
               .ToListAsync(cancellationToken);
        }

        public async Task<Tag> GetTagAsync(
        string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
                .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
        }

        public async Task<IList<AuthorItem>> ListAuthorAsync(int N, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
           .Select(x => new AuthorItem()
           {
               Id = x.Id,
               FullName = x.FullName,
               UrlSlug = x.UrlSlug,
               ImageUrl = x.ImageUrl,
               JoinedDate = x.JoinedDate,
               Email = x.Email,
               Notes = x.Notes,
               PostCount = x.Posts.Count(p => p.Published)
           })
           .OrderByDescending(x => x.PostCount)
           .Take(N)
           .ToListAsync(cancellationToken);
        }

        public async Task<IList<TagItem>> GetTagsAsync(
        CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
                .OrderBy(x => x.Name)
                .Select(x => new TagItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    PostCount = x.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }

        //Cau 1s -- lab02 -- CAN HOI THEM     

        private IQueryable<Post> FilterPosts(PostQuery condition)
        {
            IQueryable<Post> posts = _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Tags);

            if (condition.PublishedOnly)
            {
                posts = posts.Where(x => x.Published);
            }

            if (condition.NotPublished)
            {
                posts = posts.Where(x => !x.Published);
            }

            if (condition.CategoryId > 0)
            {
                posts = posts.Where(x => x.CategoryId == condition.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
            {
                posts = posts.Where(x => x.Category.UrlSlug.Contains(condition.CategorySlug));
            }

            if (condition.AuthorId > 0)
            {
                posts = posts.Where(x => x.AuthorId == condition.AuthorId);
            }

            if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
            {
                posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
            }

            if (!string.IsNullOrWhiteSpace(condition.TagSlug))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
            }

            if (!string.IsNullOrWhiteSpace(condition.Keyword))
            {
                posts = posts.Where(x => x.Title.Contains(condition.Keyword) ||
                                         x.ShortDescription.Contains(condition.Keyword) ||
                                         x.Description.Contains(condition.Keyword) ||
                                         x.Category.Name.Contains(condition.Keyword) ||
                                         x.Tags.Any(t => t.Name.Contains(condition.Keyword)));
            }

            if (condition.Year > 0)
            {
                posts = posts.Where(x => x.PostedDate.Year == condition.Year);
            }

            if (condition.Month > 0)
            {
                posts = posts.Where(x => x.PostedDate.Month == condition.Month);
            }

            if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
            {
                posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
            }

            return posts;
        }

        public async Task<bool> DeletePostAsync(int postId, CancellationToken cancellationToken = default)
        {
            var post = await _context.Set<Post>().FindAsync(postId);

            if (post is null) return false;

            _context.Set<Post>().Remove(post);
            var rowsCount = await _context.SaveChangesAsync(cancellationToken);

            return rowsCount > 0;
        }
    }
}
