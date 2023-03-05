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
    Task<IList<Category>> GetCatergoriesAsync(
        bool showOnMenu = false,
        CancellationToken cancellationToken= default);
    Task<List<CategoryItem>> GetCategoriesAsync(
        bool showOnMenu = false,
        CancellationToken cancellationToken = default);

    Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams,
        CancellationToken cancellationToken = default);

    //tim 1 the (tag) co ten dinh danh la slug
    Task<Tag> GetTagAsyn(
       string slug,
       CancellationToken cancellationToken = default);

    //Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó.
    //Kết quả trả về kiểu IList<TagItem>
    Task<IList<TagItem>> GetListTagAsync(string tag,
            CancellationToken cancellationToken = default);

    //Xóa một thẻ theo mã cho trước.
    Task DeleteTagByIDAcsyn(int? id,
            CancellationToken cancellationToken = default);
}
