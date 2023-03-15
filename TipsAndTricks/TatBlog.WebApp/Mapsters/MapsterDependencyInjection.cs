using Mapster;
using MapsterMapper;

namespace TatBlog.WebApp.Mapsters;

public static class MapsterDependencyInjection
{
    //thêm các dịch vụ cần thiết của Mapster vào DI Container
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
