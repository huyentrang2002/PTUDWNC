//b.Tạo lớp DTO có tên là TagItem để chứa các thông tin về thẻ và số lượng bài viết chứa thẻ đó.
namespace TatBlog.Core.DTO
{
    public class TagItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public string Description { get; set; }
        public int PostCount { get; set; }
    }
}
