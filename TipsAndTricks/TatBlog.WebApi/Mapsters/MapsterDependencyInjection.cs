using Mapster;
using MapsterMapper;

namespace TatBlog.WebApi.Mapsters;

//định nghĩa hàm để đăng ký Mapster với DI Container
public static class MapsterDependencyInjection
{
    public static WebApplicationBuilder ConfigureMapster(
        this WebApplicationBuilder builder)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapsterConfiguration).Assembly);

        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();

        return builder;
    }
}
