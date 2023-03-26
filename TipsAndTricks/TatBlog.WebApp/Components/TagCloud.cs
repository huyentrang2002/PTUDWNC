using Microsoft.AspNetCore.Mvc;
using TatBlog.Service.Blogs;


namespace TatBlog.WebApp.Components

{
    public class TagCloud : ViewComponent
    {

        public readonly IBlogRepository _blogRepository;

        public TagCloud(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Hiển thi danh sách các thẻ tag
            var tagCloud = await _blogRepository.GetTagsAsync();
            return View(tagCloud);


        }

    }
}
