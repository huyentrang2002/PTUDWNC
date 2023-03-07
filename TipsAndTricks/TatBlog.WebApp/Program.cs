using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Service.Blogs;
using TatBlog.WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.ConfigureMVC()
        .ConfigureServices();
}

var app = builder.Build();
{
    app.UseRequestPipeLine();
    app.UseBlogRoutes();
    app.UseDataSeeder();    

}

app.Run();

