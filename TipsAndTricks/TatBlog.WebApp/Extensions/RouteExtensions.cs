using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace TatBlog.WebApp.Extensions;

public static class RouteExtensions
{
    public static IEndpointRouteBuilder UseBlogRoutes(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllerRoute(
        name: "post-by-category",
        pattern: "blog/category/{slug}",
        defaults: new { controller = "Blog", action = "Catergory" });

        endpoints.MapControllerRoute(
            name: "post-by-tag",
            pattern: "blog/tag/{slug}",
            defaults: new { controller = "Blog", action = "Tag" });


        endpoints.MapControllerRoute(
        name: "post-by-author",
        pattern: "blog/author/{slug}",
        defaults: new { controller = "Blog", action = "Author" });

        endpoints.MapControllerRoute(
            name: "single-post",
            pattern: "blog/post/{year:int}/{month:int}/{day:int}/{slug}",
            defaults: new { controller = "Blog", action = "Post" });

        //khu vực quản trị
        endpoints.MapControllerRoute(
            name: "admin-area",
            pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}",
            defaults: new { area = "Admin" });


        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Blog}/{action=Index}/{id?}");

        return endpoints;
    }
}
