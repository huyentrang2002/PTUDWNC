using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Service.Blogs;
using TatBlog.Service.Media;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Model;


namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class PostsController : Controller
{
    
    private readonly ILogger<PostsController> _logger;
    private readonly IBlogRepository _blogRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IMediaManager _mediaManager;
    private readonly IMapper _mapper;


    public PostsController(
        ILogger<PostsController> logger,
        IBlogRepository blogRepository,
        IAuthorRepository authorRepository,
        IMediaManager mediaManager,
        IMapper mapper)
    {
        _logger = logger;
        _blogRepository = blogRepository;
        _authorRepository = authorRepository;
        _mediaManager = mediaManager;
        _mapper = mapper;
    }

    // gán giá trị cho các attribute cua đối tượng PostFilterModel
    private async Task PopulatePostFilterModeAsync(PostFilterModel model)
    {
        var authors = await _authorRepository.GetAuthorsAsync();
        var categories = await _blogRepository.GetCategoriesAsync();

        model.AuthorList = authors.Select(a => new SelectListItem()
        {
            Text = a.FullName,
            Value = a.Id.ToString()
        });

        model.CategoryList = categories.Select(c => new SelectListItem()
        {
            Text = c.Name,
            Value = c.Id.ToString()
        });
    }
    public async Task<IActionResult> Index(
        PostFilterModel model,
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = 10)
    {
        _logger.LogInformation("tạo điều kiện truy vấn");

       

        //sử dụng mapster để tạo đối tượng PostQuery từ đối tượng PostFilterModel model
        var postQuery = _mapper.Map<PostQuery>(model);

        _logger.LogInformation("lấy danh sách bài viết từ cơ sở dũ liệu");

        //var postQuery = new PostQuery()
        //{
        //    Keyword = model.Keyword,
        //    CategoryId = model.CategoryId,
        //    AuthorId = model.AuthorId,
        //    Year = model.Year,
        //    Month = model.Month
        //};
        ViewBag.PostList = await _blogRepository
            .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

        _logger.LogInformation("chuẩn bị cơ sở dũ liệu cho ViewModel");

        await PopulatePostFilterModeAsync(model);
        return View(model);
    }

    //xử lý yêu cầu thêm mới
    //cập nhật một bài viết có mã số(ID) cho trước
    [HttpGet]
    public async Task<IActionResult> Edit(int id = 0)
    {
        //ID = 0 -> Thêm bài viết mới
        //ID > 0 -> Đọc dữ liệu của bài viết từ CSDL
        var post = id > 0 ? await _blogRepository.GetPostByIdAsync(id, true) : null;

        //tạo view modle từ dữ liệu của bài viết
        var model = post == null ? new PostEditModel()
            : _mapper.Map<PostEditModel>(post);

        //gán các giá trị khác cho view model
        await PopulatePostEditModelAsync(model);

        return View(model);

    }
    public async Task<IActionResult> TogglePub(int id = 0)
    {
         await _blogRepository.TogglePublishedFlagAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id = 0)
    {
        await _blogRepository.DeletePostAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulatePostEditModelAsync(PostEditModel model)
    {
        var authors = await _authorRepository.GetAuthorsAsync();
        var categories = await _blogRepository.GetCategoriesAsync();

        model.AuthorList = authors.Select(a => new SelectListItem
        {
            Text = a.FullName,
            Value = a.Id.ToString()
        });

        model.CategoryList = categories.Select(c => new SelectListItem
        {
            Text = c.Name,
            Value = c.Id.ToString()
        });
    }

    //xử lý việc lưu các thay đổi mà người dùng đã nhập vào
    [HttpPost]
    public async Task<IActionResult> Edit(
        [FromServices] IValidator<PostEditModel> postValidator,
        PostEditModel model)
    {
        // sử dụng FluentValidation cho việc kiểm tra dữ liệu đầu vào
        var validationResult = await postValidator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
        }

        if (!ModelState.IsValid)
        {
            await PopulatePostEditModelAsync(model);
            return View(model);
        }

        var post = model.Id > 0 ? await _blogRepository
            .GetPostByIdAsync(model.Id, true) : null;

        if (post == null)
        {
            post = _mapper.Map<Post>(model);

            post.Id = 0;
            post.PostedDate = DateTime.Now;
        }
        else
        {
            _mapper.Map(model, post);

            post.Category = null;
            post.ModifiedDate = DateTime.Now;
        }

        //nếu người dùng có uploadhinhf anh minh hoa cho bai viết
        if (model.ImageFile?.Length > 0)
        {
            //thì thực hiện việc lưu tập tin vào thư mục uploads
            var newImagePath = await _mediaManager.SaveFileAsync(
                model.ImageFile.OpenReadStream(),
                model.ImageFile.FileName,
                model.ImageFile.ContentType);

            //nếu lưu thành công, xóa tập tin hình ảnh cũ(nếu có)
            if (!string.IsNullOrWhiteSpace(newImagePath))
            {
                await _mediaManager.DeleteFileAsync(post.ImageUrl);
                post.ImageUrl = newImagePath;
            }
        }

        await _blogRepository.CreateOrUpdatePostAsync(
            post, model.GetSelectedTags());

        return RedirectToAction(nameof(Index));
    }

    

    // kiểm tra xem UrlSlug đã được sử dụng cho một bài viết khác hay chưa
    [HttpPost]
    public async Task<IActionResult> VerifyPostSlug(int id, string slug)
    {
        var slugExisted = await _blogRepository
            .IsPostSlugExistAsync(id, slug);

        return slugExisted
            ? Json($"Slug '{slug}' đã được sử dụng")
            : Json(true);
    }

}
