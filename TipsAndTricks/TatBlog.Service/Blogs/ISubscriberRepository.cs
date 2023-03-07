using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface ISubscriberRepository
{
    // Đăng ký theo dõi: SubscribeAsync(email)
    Task SubscribeAsync(string email);

    // Hủy đăng ký: UnsubscribeAsync(email, reason, voluntary)
    Task UnsubscribeAsync(string email, string reason, bool voluntary);

    // Chặn một người theo dõi: BlockSubscriberAsync(id, reason, notes)
    Task BlockSubscriberAsync(int id, string reason, string notes);

    // Xóa một người theo dõi: DeleteSubscriberAsync(id)
    Task DeleteSubscriberAsync(int id);

    // Tìm người theo dõi bằng ID: GetSubscriberByIdAsync(id)
    Task<Subscriber> GetSubscriberByIdAsync(int id);

    // Tìm người theo dõi bằng email: GetSubscriberByEmailAsync(email)
    Task<Subscriber> GetSubscriberByEmailAsync(string email);

    //Tìm danh sách người theo dõi theo nhiều tiêu chí khác nhau, kết quả ]
    //ược phân trang: Task<IPagedList<Subscriber>>
    //SearchSubscribersAsync(pagingParams, keyword, unsubscribed,involuntary).
}