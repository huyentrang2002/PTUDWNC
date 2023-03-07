using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Service.Blogs;
using TatBlog.Service.Extensions;
public class AuthorRepository : IAuthorRepository
{
    private readonly BlogDbContext _context;
    public AuthorRepository(BlogDbContext context)
    {
        _context = context;
    }

    //2b.Tìm một tác giả theo mã số.
    public async Task<Author> GetAuthorByIdAsync(int id, 
        CancellationToken cancellationToken)
    {
        var author = await _context.Authors
            .FindAsync(id, cancellationToken);
        return author;
    }
    
    //2c. Tìm một tác giả theo tên định danh (slug).
    public async Task<Author> GetAuthorBySlugAsync(string slug, 
        CancellationToken cancellationToken)
    {
        var tagQuery =  _context.Authors
            .FindAsync(slug, cancellationToken);
        return await tagQuery;
    }

    //2d. Lấy và phân trang danh sách tác giả kèm theo số lượng bài viết của tác giả đó.Kết quả trả về kiểu IPagedList<AuthorItem>.
    public async Task<IPagedList<AuthorItem>> GetAuthorsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        var tagQuery = _context.Set<Author>()
                                  .Select(x => new AuthorItem()
                                  {
                                      Id = x.Id,
                                      FullName = x.FullName,
                                      UrlSlug = x.UrlSlug,
                                      ImageUrl = x.ImageUrl,
                                      JoinedDate = x.JoinedDate,
                                      Notes = x.Notes,
                                      PostCount = x.Posts.Count(p => p.Published)
                                  });

        return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
    }
    //2e. Thêm hoặc cập nhật thông tin một tác giả
    public async Task AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default)
    {
        var auth = await _context.Authors
            .FirstOrDefaultAsync(a => a.Id == author.Id);
        
        auth.FullName = author.FullName;
        auth.UrlSlug = author.UrlSlug;
        auth.JoinedDate = author.JoinedDate;
        auth.Email = author.Email;
        auth.Notes = author.Notes;

        _context.Attach(auth).State = EntityState.Modified;
         await _context.SaveChangesAsync();
    }

    //2f. Tìm danh sách N tác giả có nhiều bài viết nhất. N là tham số đầu vào



}
