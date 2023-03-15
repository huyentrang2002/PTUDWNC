using FluentValidation;
using TatBlog.Service.Blogs;
using TatBlog.WebApp.Areas.Admin.Model;

namespace TatBlog.WebApp.Validations;

public class PostValidator : AbstractValidator<PostEditModel>
{
    private readonly IBlogRepository _blogRepository;

    public PostValidator(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.ShortDescription)
            .NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.Meta)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.UrlSlug)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.UrlSlug)
            .MustAsync(async (postModel, slug, cancellationToken) =>
            !await blogRepository.IsPostSlugExistAsync(
                postModel.Id, slug, cancellationToken))
            .WithMessage("Slug {PropertyValue} da duoc su dung");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn chủ đề của bài viết");

        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn tac gia của bài viết");

        RuleFor(x => x.SelectedTags)
            .NotEmpty()
            .WithMessage("Bạn phải nhập ít nhất 1 thẻ");

        When(x => x.Id <= 0, () =>
        {
            RuleFor(x => x.ImageFile)
            .Must(x => x is { Length: > 0 })
            .WithMessage("Bạn phải chọn hình anh cho bài viết");
        })
        .Otherwise(() =>
        {
            RuleFor(x => x.ImageFile)
            .MustAsync(SetImageIfMotExist)
            .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
        });

    }

    //kiểm tra người dùng đã nhập ít nhất 1 thẻ(tag)
    private bool HasAtLeastOneTag(PostEditModel postModel, string selectedTags)
    {
        return postModel.GetSelectedTags().Any();
    }

    //kiểm tra bài viết đã có hình ảnh hay chưa
    //Nếu chưa có bắt buộc người dùng phải chon file
    private async Task<bool> SetImageIfMotExist(
        PostEditModel postModel,
        IFormFile imageFile,
        CancellationToken cancellationToken)
    {
        //lấy thong tin bài viết từ cơ sỏ dữ liệu
        var post = await _blogRepository.GetPostByIdAsync(
            postModel.Id, false, cancellationToken);

        //nếu bài viết đã có hình ảnh -> không bắt buộc chọn file
        if (!string.IsNullOrWhiteSpace(post?.ImageUrl))
            return true;

        //ngược lại (bài viết chưa có hình ảnh) -> kiểm tra
        //nếu người dùng chưa chọn file thì báo lỗi
        return imageFile is { Length: > 0 };

    }
}

