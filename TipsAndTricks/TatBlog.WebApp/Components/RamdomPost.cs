using Microsoft.AspNetCore.Mvc;
using TatBlog.Service.Blogs;


namespace TatBlog.WebApp.Components

{
    public class RandomPosts : ViewComponent
    {

        public readonly IBlogRepository _blogRepository;

        public RandomPosts(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Lấy 5 bài viết ngẫu nhiên 
            var ramdomPosts = await _blogRepository.GetRandomPostAsync(5);
            return View(ramdomPosts);


        }

    }
}
