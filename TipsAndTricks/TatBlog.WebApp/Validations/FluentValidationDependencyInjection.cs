using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace TatBlog.WebApp.Validations;

public static class FluentValidationDependencyInjection
{
    //đăng ký các dịch vụ được sử dụng bởi
    //FluentValidation với DI container

    public static WebApplicationBuilder ConfigureFluentValidation(
        this WebApplicationBuilder builder)
    {
        //enable client-side integration
        builder.Services.AddFluentValidationClientsideAdapters();
        //sacn ang register all validators in given assembly
        builder.Services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly());

        return builder;
    }
}
