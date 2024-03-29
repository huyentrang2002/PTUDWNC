﻿using Microsoft.AspNetCore.Mvc;
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

        //endpoints.MapControllerRoute(
        //    name: "single-post",
        //    pattern: "blog/post/{year:int}/{month:int}/{day:int}/{slug}",
        //    defaults: new { controller = "Blog", action = "Post" });
        endpoints.MapControllerRoute(
              name: "post-info",
              pattern: "blog/postinfo/{year:int}/{month:int}/{day:int}/{slug}",
              defaults: new { controller = "Blog", action = "PostInfo" });

        //endpoints.MapControllerRoute(
        //    name: "admin-area",
        //    pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}",
        //    defaults: new { area = "Admin" });

        //featured
        endpoints.MapControllerRoute(
            name: "featured-post",
            pattern: "blog/featuredposts/{slug}/{view}",
            defaults: new { controller = "Blog", action = "FeaturedPosts" });


        endpoints.MapControllerRoute(
            name: "random-post",
            pattern: "blog/featuredposts/{slug}/{view}",
            defaults: new { controller = "Blog", action = "RandomPosts" });

        
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Blog}/{action=Index}/{id?}");

        return endpoints;
    }
}
