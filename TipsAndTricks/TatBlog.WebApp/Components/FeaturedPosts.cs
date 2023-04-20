using Microsoft.AspNetCore.Mvc;
using TatBlog.Service.Blogs;


namespace TatBlog.WebApp.Components
    
{
    public class FeaturedPosts : ViewComponent
    {

        public readonly IBlogRepository _blogRepository;

        public FeaturedPosts(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy top bài viết dược xem nhiều nhất
            var featuredPosts = await _blogRepository.GetPopularArticAsync(3);
            return View(featuredPosts);

            //Lấy 5 bài viết ngẫu nhiên 
            var ramdomPosts = await _blogRepository.GetRandomPostAsync(5);
            return View(ramdomPosts);


        }

    }
}
