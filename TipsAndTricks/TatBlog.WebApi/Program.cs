using TatBlog.WebApi.Endpoints;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Mapsters;
using TatBlog.WebApi.Validations;

var builder = WebApplication.CreateBuilder(args);
{
    //Add servicestothe container
    builder
        .ConfigureCos()
        //.ConfigureNLog()
        .ConfigureServices()
        .ConfigureSwaggerOpenApi()
        .ConfigureMapster()
        .ConfigureFluentValidation();
}
var app = builder.Build();
{
    //configure the HTTP request pineline
    app.SendRequestPipeline();

    //Configure API endpoints
    app.MapAuthorEnpoints();

    app.Run();
}


