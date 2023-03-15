using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TatBlog.WebApp.Areas.Admin.Model;
public class PostEditModel
{
    [DisplayName("Tiêu đề")]
    public int Id { get; set; }
    public string Title { get; set; }

    [DisplayName("Giới thiệu")]
    public string ShortDescription { get; set; }

    [DisplayName("Nội dung")]
    public string Description { get; set; }

    [DisplayName("Metadata")]
    public string Meta { get; set; }

    public string UrlSlug { get; set; }

    [DisplayName("Chọn hình ảnh")]
    public IFormFile ImageFile { get; set; }

    [DisplayName("Hình ảnh hiện tại")]
    public string ImageUrl { get; set; }

    [DisplayName("Xuất bản ngay")]
    public bool Published { get; set; }

    [DisplayName("Chủ đề")]
    public int CategoryId { get; set; }

    [DisplayName("Tác giả")]
    public int AuthorId { get; set; }

    [DisplayName("Từ khóa(mỗi từ 1 dòng)")]
    public string SelectedTags { get; set; }

    public IEnumerable<SelectListItem> AuthorList { get; set; }
    public IEnumerable<SelectListItem> CategoryList { get; set; }


    //tách chuỗi chứa các thẻ thành 1 mảng có chuỗi
    public List<string> GetSelectedTags()
    {
        return (SelectedTags ?? "")
            .Split(new[] { ',', ';', '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }


}
