using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities;

public class Author:IEntity
{
    //ma tac gia --> ke thua interface
    public int Id { get; set; }

    //ten tac gia
    public string FullName { get; set; }

    //Ten dinh danh tao URL
    public string UrlSlug { get; set; }

    //duong dan toi file hinh anh
    public string ImageUrl { get; set; }

    //ngay bat dau
    public DateTime JoinedDate{ get; set; }

    //email
    public string Email { get; set; }

    //ghi chu
    public string Notes { get; set; }

    //Danh sach ca bai viet cua tac gia
    public IList<Post> Posts { get; set; }

}
