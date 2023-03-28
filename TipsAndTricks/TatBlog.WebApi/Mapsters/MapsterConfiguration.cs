using Mapster;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Service.Blogs;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Mapsters;

//quy định cách sao chép dữ liệu giữa các đối tượng
public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Tac gia
        config.NewConfig<Author, AuthorDto>();
        config.NewConfig<Author, AuthorItem>()
            .Map(dest => dest.PostCount,
            src => src.Posts == null ? 0 : src.Posts.Count);
        config.NewConfig<AuthorEditModel, AuthorItem>();

        //Chu de
        config.NewConfig<Category, CategoryDto>();
        config.NewConfig<Category, CategoryItem>()
            .Map(dest => dest.PostCount,
            src => src.Posts == null ? 0 : src.Posts.Count);

        //bai viet
        config.NewConfig<Post, PostDto>();
        config.NewConfig<Post, PostDetail>();
    }
}
