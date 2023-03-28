using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Service.Blogs;
using TatBlog.Service.Media;
using TatBlog.Services.Blogs;
using TatBlog.Services.Timing;


namespace TatBlog.WebApi.Extensions;
public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();

        builder.Services.AddDbContext<BlogDbContext>(option =>
            option.UseSqlServer(
                builder.Configuration
                .GetConnectionString("DefaultConnection")));

        builder.Services
            .AddScoped<ITimeProvider, LocalTimeProvider>();
        builder.Services
            .AddScoped<IMediaManager, LocalFileSystemMediaManager>();
        builder.Services
            .AddScoped<IBlogRepository, BlogRepository>();
        builder.Services
            .AddScoped<IAuthorRepository, AuthorRepository>();

        return builder;
    }

    public static WebApplicationBuilder ConfigureCos(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(option =>
        {
            option.AddPolicy("TatBlogApp",
                policyBuiler => policyBuiler
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
        return builder;
    }

    // Cau hinh viec su dung Nlog
    //public static WebApplicationBuilder ConfigureNLog(
    //    this WebApplicationBuilder builder)
    //{
    //    builder.Logging.ClearProviders();
    //    builder.Host.UseNLog();

    //    return builder;
    //}

    public static WebApplicationBuilder ConfigureSwaggerOpenApi(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplication SendRequestPipeline(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseCors();
        
        return app; 
    }

}
