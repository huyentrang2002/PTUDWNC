using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations;
//cài đặt các quy tắc kiểm tra dữ liệu nhập về thông tin bài viết.
public class PostValidator : AbstractValidator<PostEditModel>
{
	public PostValidator()
	{
		RuleFor(a => a.Title)
			.NotEmpty()
			.WithMessage("ten tieu de khong duoc de trong")
			.MaximumLength(100)
			.WithMessage("ten tac gia toi da 100 ky tu");

		RuleFor(a => a.ShortDescription)
				.NotEmpty()
				.WithMessage("Description khong duoc de trong")
				.MaximumLength(100)
				.WithMessage("Description chua toi da 100 ky tu ");

		RuleFor(a => a.Description)
		   .NotEmpty()
		   .WithMessage("Description khong duoc de trong")
		   .MaximumLength(100)
		   .WithMessage("Description chua toi da 100 ky tu ");

	}
}
