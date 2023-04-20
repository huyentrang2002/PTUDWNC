namespace TatBlog.WebApi.Models;
public class PostDetail
{
    public bool Pulished { get; set; }
    //Ma bai viet
    public int Id { get; set; }

    //Tieu de bai viet
    public string Title { get; set; }

    //Mo ta hay gioi thieu ngan ve noi dung
    public string ShortDescription { get; set; }

    //noi dung chi tiet cua bai viet
    public string Description { get; set; }

    //Metadata
    public string Meta { get; set; }

    //Ten dinh danh de tao URL
    public string UrlSlug { get; set; }

    //Duong dan toi tap tin hinh anh
    public string ImageUrl { get; set; }

    //So luong nguoi xem, doc bai viet
    public int ViewCount { get; set; }

    //Ngay gio dang bai
    public DateTime PostedDate { get; set; }

    //ngay gio cap nhat la cuoi
    public DateTime? ModifiedDate { get; set; }

    //Chuyen muc bai viet
    public CategoryDto Category { get; set; }

    //Tac gia cua bai viet
    public AuthorDto Author { get; set; }

    //Danh sach cac tu khoa cua bai viet
    public IList<TagDto> Tags { get; set; }



}
