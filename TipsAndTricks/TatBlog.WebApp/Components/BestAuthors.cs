using Microsoft.AspNetCore.Mvc;
using TatBlog.Service.Blogs;


namespace TatBlog.WebApp.Components

{
    public class BestAuthor : ViewComponent
    {

        public readonly IBlogRepository _blogRepository;

        public BestAuthor(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Hiển thi danh sách các thẻ tag
            var bestAuthor = await _blogRepository.ListAuthorAsync(4);
            return View(bestAuthor);


        }

    }
}
