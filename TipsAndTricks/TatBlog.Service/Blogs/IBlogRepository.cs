using TatBlog.Core.Entities;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;

namespace TatBlog.Service.Blogs;

public interface IBlogRepository
{
    //tim bai viet co ten dinh danh la 'slug'
    //duoc dang vao thang, nam
    Task<Post> GetPostAsyn(
        int year,
        int month,  
        string slug,
        CancellationToken cancellationToken = default);

    //tim top n bai viet pho duoc nhieu nguoi xem nhat
    Task<IList<Post>> GetPopularArticAsync(
        int numPosts,
        CancellationToken cancellationToken = default);

    //kiem tra xem ten dinh danh cua bai viet da co hay chua
    Task<bool> IsPostSlugExistAsync(
        int postId, string slug,
        CancellationToken cancellationToken = default);

    //tang so luong nguoi xem cua 1 bai viet
    Task IncreaseViewCountAsync(
        int postId,
        CancellationToken cancellationToken = default);

    //lay danh sach chuyen muc va so luong bai viet
    //nam thuoc tung chuyen muc/ chu de
    Task<IList<Category>> GetCategoriesAsync(
        bool showOnMenu = false,
        CancellationToken cancellationToken= default);
    Task<List<CategoryItem>> GetCategoryItemsAsync(
        bool showOnMenu = false,
        CancellationToken cancellationToken = default);

    Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams,
        CancellationToken cancellationToken = default);

    Task<Post> CreateOrUpdatePostAsync(
        Post post, IEnumerable<string> tags,
        CancellationToken cancellationToken = default);
    Task<Category> CreateOrUpdateCategoryAsync(
        Category category, CancellationToken cancellationToken = default);

    //tim 1 the (tag) co ten dinh danh la slug
    Task<Tag> GetTagAsync(
       string slug,
       CancellationToken cancellationToken = default);

    //Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó.
    //Kết quả trả về kiểu IList<TagItem>
    Task<IList<TagItem>> GetListTagAsync(string tag,
            CancellationToken cancellationToken = default);

    //Xóa một thẻ theo mã cho trước.
    Task DeleteTagByIDAcsyn(int? id,
            CancellationToken cancellationToken = default);

    //Tìm một chuyên mục theo mã số cho trước.
    Task<Category> GetCategoryByIdAsync
        (int id, CancellationToken cancellationToken = default);

    //Thêm hoặc cập nhật một chuyên mục/chủ đề.
    Task AddOrUpdateCategoryAsync(Category category,
        CancellationToken cancellationToken = default);

    //Xóa một chuyên mục theo mã số cho trước
    Task DeleteCategoryByIDAcsyn(int? id,
            CancellationToken cancellationToken = default);

    //Kiểm tra tên định danh (slug) của một chuyên mục đã tồn tại hay chưa
    Task<bool> CheckCategorySlugExisted(string slug);

    Task<Category> GetCategoriesBySlugAsync(string slug,
            CancellationToken cancellationToken = default);

    //Lấy và phân trang danh sách chuyên mục, kết quả trả về kiểu IPagedList<CategoryItem
    Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync
        (IPagingParams pagingParams, CancellationToken cancellationToken = default);

    //k. Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào. Kết 
    //quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số
    //bài viết.
    Task<int> CountPostsAsync(
       PostQuery condition, CancellationToken cancellationToken = default);
    Task<IList<PostInMonthItem>> CountMonthlyPostsAsync(
            int numMonths, CancellationToken cancellationToken = default);

    //Tìm một bài viết theo mã số
    Task<Post> GetPostByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Post> GetPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default);

    //Thêm hay cập nhật một bài viết
    Task AddOrUpdatePostAsync(Post post, CancellationToken cancellationToken = default);

    //Chuyển đổi trạng thái Published của bài viết
    Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default);

    //Lấy ngẫu nhiên N bài viết. N là tham số đầu vào
    Task<IList<Post>> GetRandomPostAsync(int n, CancellationToken cancellationToken = default);

    //CODE MAU    Task<bool> TogglePublishedFlagAsync(
        int postId, CancellationToken cancellationToken = default);
    //Task<Author> GetAuthorAsync(string slug, CancellationToken cancellationToken = default);
    //Task<Author> GetAuthorByIdAsync(int authorId);
    //Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default);
    Task<Tag> GetTagAsync(
        string slug, CancellationToken cancellationToken = default);

    Task<IList<AuthorItem>> ListAuthorAsync(int N, CancellationToken cancellationToken = default);

    Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default);

    Task<IList<Post>> GetPostsAsync(PostQuery condition, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<IPagedList<Post>> GetPagedPostsAsync(
        PostQuery condition,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetPagedPostsAsync<T>(
            PostQuery condition,
            IPagingParams pagingParams,
            Func<IQueryable<Post>, IQueryable<T>> mapper);
    Task<bool> DeletePostAsync(
       int postId, CancellationToken cancellationToken = default);




}




