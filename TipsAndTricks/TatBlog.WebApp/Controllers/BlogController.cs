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
    
    #region TRANG CHỦ
    public async Task<IActionResult> Index(
        [FromQuery(Name = "k")] string keywork = null,
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = 10)
    {
        //tao doi tuong chua cac dieu kien truy van 
        var postQuery = new PostQuery()
        {
            //chi lay nhung bai viet co trang thai Pushlished
            PublishedOnly = true,

            //tim bai viet theo tu khoa
            Keyword = keywork
        };
        //Console.WriteLine("Loggggg");
        //Truy van cac bai viet theo dieu kien da tao
        var postsList = await _blogRepository
            .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

        //luu lai dieu kien truy van de hien thi trong View
        ViewBag.PostQuery = postQuery;

        //truyen danh sach bai viet vao view de render ra HTML
        return View(postsList);
    }
    #endregion

    #region Tìm Category theo slug
    public async Task<IActionResult> Category(
        string slug = null,
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = 10)
    {
        var postQuery = new PostQuery()
        {
            
            PublishedOnly= true,
            CategorySlug = slug
        };

        //truy van bai viet theo dieu kien da tao
        var categories = await _blogRepository
            .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

        //luu lai dieu kien truy van de hien thi trong View
        ViewBag.PostQuery = postQuery;

        return View(categories);
    }
    #endregion

    #region Tìm Author theo slug
    public async Task<IActionResult> Author(
        [FromRoute(Name = "slug")] string slug = null,
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = 10)
    {
        var postQuery = new PostQuery()
        {
            //trang thái hiển thị
            PublishedOnly = true,
            AuthorSlug = slug
        };

        //truy van tac gia theo dieu kien da tao
        var authors = await _blogRepository
            .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

        //luu lai dieu kien truy van de hien thi trong View
        ViewBag.PostQuery = postQuery;

        return View(authors);
    }
    #endregion

    #region Tìm Tag theo slug
    public async Task<IActionResult> Tag(
        [FromRoute(Name = "slug")] string slug = null,
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = 10)
    {
        var postQuery = new PostQuery()
        {
            //trang thái hiển thị
            PublishedOnly = true,
            TagSlug = slug
        };

        //truy van the theo dieu kien da tao
        var tags = await _blogRepository
            .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

        //luu lai dieu kien truy van de hien thi trong View
        ViewBag.PostQuery = postQuery;

        return View(tags);
    }
    #endregion

    #region Tìm Post theo slug
    //public async Task<IActionResult> Post(
    //    [FromRoute(Name = "year")] int year,
    //    [FromRoute(Name = "month")] int month,
    //    [FromRoute(Name = "slug")] string slug,
    //    [FromQuery(Name = "p")] int pageNumber = 1,
    //    [FromQuery(Name = "ps")] int pageSize = 10)
    //{
    //    var postQuery = new PostQuery()
    //    {
    //        //trang thái hiển thị
    //        PublishedOnly = true,
    //        PostYear = year,
    //        PostMonth = month,
    //        Slug = slug
    //    };

    //    //truy van tac gia theo dieu kien da tao
    //    var tags = await _blogRepository
    //        .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

    //    //luu lai dieu kien truy van de hien thi trong View
    //    ViewBag.PostQuery = postQuery;

    //    return View(tags);
    //}
    #endregion

    #region hiển thị post được đăng theo thang/nam

    public async Task<IActionResult> PostInfo(
    [FromRoute(Name = "year")] int year,
    [FromRoute(Name = "month")] int month,
    [FromRoute(Name = "day")] int day,
    [FromRoute(Name = "slug")] string slug = null)

    {
    //    var postQuery = new PostQuery()
    //    {
    //        //trang thái hiển thị
    //        PublishedOnly = true,
    //        TagSlug = slug
    //    };

        //truy van bau viet theo dieu kien da tao
        var postList = await _blogRepository
            .GetPostAsyn(year, month, slug);
      //  ViewBag.PostQuery = postQuery;

        return View(postList);

    }
    #endregion






    //====TẠO CONTROLLER VÀ ACTION====

    public IActionResult About()
        =>View();

    public IActionResult Contact()
        => View();

    
    public IActionResult Rss()
        => Content("Noi dung se duoc cap nhat");


}
