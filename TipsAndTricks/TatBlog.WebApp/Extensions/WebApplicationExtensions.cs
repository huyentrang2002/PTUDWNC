using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Service.Blogs;
using TatBlog.Service.Media;
using NLog.Web;
using TatBlog.WebApp.Middlewares;

namespace TatBlog.WebApp.Extensions;

public static class WebApplicationExtensions
{
    //them cac dich vu duoc yeu cau boi MVC Framework
    public static WebApplicationBuilder ConfigureMVC(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();
        builder.Services.AddResponseCompression();
        return builder;
    }

    //Cấu hình cho việc sử dụng Nlog
    public static WebApplicationBuilder ConfigureNLog(
        this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        return builder;
    }

    //dang ki dich vu voi DI Container
    public static WebApplicationBuilder ConfigureServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<BlogDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration
            .GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IMediaManager, LocalFileSystemMediaManager>();
        builder.Services.AddScoped<IBlogRepository, BlogRepository>();
        builder.Services.AddScoped<IDataSeeder, DataSeeder>();

        return builder;
    }

    //Cau hinh HTTP request pipeline
    public static WebApplication UseRequestPipeLine(this WebApplication app)
    {
        //Them middleware de hien thi thong bao loi
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Blog/Error");

            // them middleware cho viec ap dung HSTS (them header
            // Strict-Transport-Security vao HTTP Response)
            app.UseHsts();
        }

        //Them middleware de chuyen huong HTTP sang HTTPS
        app.UseHttpsRedirection();

        //Them middleware phuc vu cac yeu cau lien quan
        //toi cac tap tin noi dung tinh nhu hinh anh, css, ...
        app.UseStaticFiles();

        //Them middleware lua chon endpoint phu hop nhat
        //dexuly HTTP request
        app.UseRouting();

        //them middleware de luu vet nguoi dung
        //app.UseMiddleware<UserActivityMiddleware>();

        return app;
    }

    //them du lieu mau vao CSDL
    public static IApplicationBuilder UseDataSeeder(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        try
        {
            scope.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .Initialize();
        }
        catch (Exception ex)
        {
            scope.ServiceProvider
                .GetRequiredService<Logger<Program>>()
                .LogError(ex, "Could not insert data into database");
        }
        return app;
    }

}
