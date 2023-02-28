using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities;

public class Category : IEntity
{
    //ma chuyen muc -> ke thua interface
    public int Id { get; set; }

    //ten chuyen muc
    public string Name { get; set; }
    
    //ten dinh danh de tao URL
    public string UrlSlug { get; set; }
    
    //mo ta chuyen muc
    public string Description { get; set; }
    
    //danh dau hien thi chuyen muc
    public bool  ShowOnMenu { get; set; }

    //danh sach bai viet thuoc chuyen muc
    public IList<Post> Posts { get; set; }
    public int PostCount { get; set; }
}
