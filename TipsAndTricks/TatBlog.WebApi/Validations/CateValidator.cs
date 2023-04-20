using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations;

//cài đặt các quy tắc kiểm tra dữ liệu nhập về thông tin tác giả.
public class CateValidator : AbstractValidator<CateEditModel>
{
	public CateValidator()
	{
		RuleFor(a => a.Name)
		.NotEmpty()
		.WithMessage("Tên tác giả không được để trống")
		.MaximumLength(100)
		.WithMessage("Tên tác giả dài tối đa '{MaxLength}' kí tự");

		RuleFor(a => a.UrlSlug)
		.NotEmpty()
		.WithMessage("Slug của tác giả không được để trống")
		.MaximumLength(1000)
		.WithMessage("Slug dài tối đa '{MaxLength}' kí tự");

		RuleFor(a => a.Description)
		.NotEmpty()
		.WithMessage("Description của tác giả không được để trống")
		.MaximumLength(1000)
		.WithMessage("Description dài tối đa '{MaxLength}' kí tự");

	}
}