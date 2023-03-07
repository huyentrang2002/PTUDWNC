using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Service.Blogs;

namespace TatBlog.WebApp.Controllers;

public class BlogController : Controller
{
    private readonly IBlogRepository _blogRepository;
    public BlogController(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    //action nay xuly HTTP Request den trang chu cua ung
    //dung web hoac tim kiem bai viet theo tu khoa 

    public async Task<IActionResult> Index(
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = 10)
    {
        //tao doi tuong chua cac dieu kien truy van 
        var postQuery = new PostQuery()
        {
            //chi lay nhung bai viet co trang thai Pushlished
            PublishedOnly = true,
        };

        //Truy van cac bai viet theo dieu kien da tao
        var postsList = await _blogRepository
            .GetPagedPostsAsync(postQuery, pageNumber, pageSize);
        
        //luu lai dieu kien truy van de hien thi trong View
        ViewBag.PostQuery = postQuery;

        //truyen danh sach bai viet vao view de render ra HTML
        return View(postsList);

    }

    //====TẠO CONTROLLER VÀ ACTION====
    
    public IActionResult About()
        =>View();

    public IActionResult Contact()
        => View();

    public IActionResult Rss()
        => Content("Noi dung se duoc cap nhat");


}
