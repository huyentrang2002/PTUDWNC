using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;

namespace TatBlog.WebApi.Validations;

//đăng ký các dịch vụ của FluentValidation với DI Container
public static class FluentValidationDependencyInjection
{
	public static WebApplicationBuilder ConfigureFluentValidation(
		this WebApplicationBuilder builder)
	{
		// Scan and register all validators in given assembly
		builder.Services.AddValidatorsFromAssembly(
			Assembly.GetExecutingAssembly());

		return builder;
	}
}