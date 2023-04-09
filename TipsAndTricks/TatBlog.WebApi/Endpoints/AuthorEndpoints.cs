using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Service.Blogs;
using TatBlog.Service.Media;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class AuthorEndpoints
{
    //định nghĩa các API endpoint cho phần quản lý thông tin tác giả.
    public static WebApplication MapAuthorEnpoints(
        this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/authors");

        // Nested Map with defined specific route
        routeGroupBuilder.MapGet("/", GetAuthors)
                         .WithName("GetAuthors")
                         .Produces<ApiResponse<PaginationResult<AuthorItem>>>();

        routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetails)
            .WithName("GetAuthorById")
            .Produces<ApiResponse<AuthorItem>>();
            //.Produces(404);

        routeGroupBuilder.MapGet(
            "/{slug:regex(^[a-z0-9 -]+$)}/posts",
            GetPostsByAuthorsSlug)
            .WithName("GetPostsByAuthorsSlug")
            .Produces<ApiResponse<PaginationResult<PostDto>>>();

        routeGroupBuilder.MapPost("/", AddAuthor)
            .WithName("AddAuthor")
			.AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
			//.Produces(201)
			//.Produces(401)
			//.Produces(409)
			.Produces<ApiResponse<AuthorItem>>();

        routeGroupBuilder.MapPost("{id:int}/avatar", SetAuthorPicture)
            .WithName("SetAuthorPicture")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<ApiResponse<string>>();
            //.Produces(400);

        routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
			.WithName("UpdateAuthor")
			//.AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
			//.Produces(204)
			//.Produces(400)
			//.Produces(409);
			.Produces<ApiResponse<string>>();


		routeGroupBuilder.MapDelete("/{id:int}", DeleteAuthor)
            .WithName("DeleteAuthor")
            //.Produces(204)
            //.Produces(404)
			.Produces<ApiResponse<string>>();

		return app;
    }

    //Xử lý yêu cầu tìm và lấy danh sách sách tác giả
    private static async Task<IResult> GetAuthors(
        [AsParameters] AuthorFilterModel model,
        IAuthorRepository authorRepository)
    {
        var authorsList = await 
            authorRepository.GetPagedAuthorsAsync(model, model.Name);

        var paginationResult =
            new PaginationResult<AuthorItem>(authorsList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }
    //Xem chi tiết tác giả
    private static async Task<IResult> GetAuthorDetails(
        int id, 
        IAuthorRepository authorRepository,
        IMapper mapper)
    {
        var author = await authorRepository.GetCachedAuthorByIdAsync(id);

        return author == null
            ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, 
            $"Không tìm thấy tac giả có mã số {id}"))
            : Results.Ok(ApiResponse.Success(mapper.Map<AuthorItem>(author)));
    }

    //Lấy bài viết theo Id của Tác giả
    private static async Task<IResult> GetPostsByAuthorId(
        int id,
        [AsParameters] PagingModel pagingModel,
        IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery()
        {
            AuthorId = id,
            PublishedOnly = true,
        };

        var postList = await blogRepository.GetPagedPostsAsync(
            postQuery, pagingModel,
            posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postList);

		return Results.Ok(ApiResponse.Success(paginationResult));

	}
	//Lay bai viet theo dinh danh slug (tac gia)
	private static async Task<IResult> GetPostsByAuthorsSlug(
        [FromRoute] string slug,
        [AsParameters] PagingModel pagingModel,
        IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery()
        {
            AuthorSlug = slug,
            PublishedOnly = true,
        };
        var postList = await blogRepository.GetPagedPostsAsync(
            postQuery, pagingModel,
            posts => posts.ProjectToType<PostDto>());
        
        var paginationResult = new PaginationResult<PostDto>(postList);

		return Results.Ok(ApiResponse.Success(paginationResult));

	}

	//Thêm tác giả
	private static async Task<IResult> AddAuthor(
        AuthorEditModel model,
        IValidator<AuthorEditModel> validator,
        IAuthorRepository authorRepository,
        IMapper mapper)
    {
        //var validationResult = await validator.ValidateAsync(model);
        //if (validationResult.IsValid)
        //{
        //    return Results.BadRequest(
        //        validationResult.Errors.ToResponse());
        //}
        if(await authorRepository
            .IsAuthorSlugExistedAsync(0, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                $"Slug '{model.UrlSlug}' đã được sử dụng"));   
        }

        var author = mapper.Map<Author>(model);
        await authorRepository.AddOrUpdateAsync(author);

        return Results.Ok(ApiResponse.Success(            
            mapper.Map<AuthorItem>(author),
            HttpStatusCode.Created));
    }

    //Thêm ảnh cho tác giả
    private static async Task<IResult> SetAuthorPicture(
        int id, IFormFile imageFile,
        IAuthorRepository authorRepository,
        IMediaManager mediaManager)
    {
        var imageUrl = await mediaManager.SaveFileAsync(
            imageFile.OpenReadStream(),
            imageFile.FileName,
            imageFile.ContentType);
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Results.Ok(ApiResponse.Fail(
                HttpStatusCode.BadRequest, "Không lưu được tập tin"));
        }

        await authorRepository.SetImageUrlAsync(id, imageUrl);
        return Results.Ok(ApiResponse.Success(imageUrl));
    }

    //Cập nhật thông tin tác giả
    private static async Task<IResult> UpdateAuthor(
        int id, AuthorEditModel model,
        IValidator<AuthorEditModel> validator,
        IAuthorRepository authorRepository,
        IMapper mapper)
    {
        //var validationResult = await validator.ValidateAsync(model);
        //if (!validationResult.IsValid)
        //{
        //    return Results.BadRequest(
        //        validationResult.Errors.ToResponse());
        //}
        if(await authorRepository
            .IsAuthorSlugExistedAsync(id, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(
                HttpStatusCode.Conflict,
                $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

        var author = mapper.Map<Author>(model);
        author.Id = id;

        return await authorRepository.AddOrUpdateAsync(author)
            ? Results.Ok(ApiResponse.Success("Author is updated",
            HttpStatusCode.NoContent))
            : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
            "Could not find author"));
    }

    // Xóa tác giả
    private static async Task<IResult> DeleteAuthor(
        int id, IAuthorRepository authorRepository)
    {
        return await authorRepository.DeleteAuthorAsync(id)
			? Results.Ok(ApiResponse.Success("Author is deleted",
			HttpStatusCode.NoContent))
			: Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
			"Could not find author"));
	}
}




