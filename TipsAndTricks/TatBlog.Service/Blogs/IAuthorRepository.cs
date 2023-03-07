using TatBlog.Core.Entities;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;

namespace TatBlog.Service.Blogs;

public interface IAuthorRepository
{
    //Tìm một tác giả theo mã số
    Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken);

    //Tìm một tác giả theo tên định danh(slug)
    Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken);

    //Lấy và phân trang danh sách tác giả kèm theo số lượng bài viết của tác giả đó.Kết quả trả về kiểu IPagedList<AuthorItem>
    Task<IPagedList<AuthorItem>> GetAuthorsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);
    

    //Thêm hoặc cập nhật thông tin một tác giả
    Task AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default);

    //Tìm danh sách N tác giả có nhiều bài viết nhất.N là tham số đầu vào




}
