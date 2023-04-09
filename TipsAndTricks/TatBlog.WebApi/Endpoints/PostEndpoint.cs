using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Service.Blogs;
using TatBlog.Service.Media;
using TatBlog.Services.Blogs;
using TatBlog.Services.Extensions;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Models;


using static System.Net.Mime.MediaTypeNames;

namespace TatBlog.WebApi.Endpoints;

public static class PostEndpoints
{
	public static WebApplication MapPostEndpoints(
		this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/posts");

		//routeGroupBuilder.MapGet("/", GetPosts)
		//	.WithName("GetPosts")
		//	.Produces<PaginationResult<PostQuery>>();

		routeGroupBuilder.MapGet("/{id:int}", GetPostDetails)
			.WithName("GetPostsByPostId")
			.Produces<PostItem>()
			.Produces(404);

		routeGroupBuilder.MapGet(
			"/{slug:regex(^[a-z0-9_-]+$)}/posts",
			GetPostsBySlug)
			.WithName("GetPostsByPostSlug")
			.Produces<PaginationResult<PostDto>>();

		routeGroupBuilder.MapPost("/", AddPost)
			.WithName("AddNewPost")
			.Accepts<PostEditModel>("multipart/form-data")
			.Produces(201)
			.Produces(400)
			.Produces(409);

		routeGroupBuilder.MapPost("/{id:int}/avatar", SetPostPicture)
		   .WithName("SetPostPicture")
		   .Accepts<IFormFile>("multipart/form-data")
		   .Produces<string>()
		   .Produces(400);

		//routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
		//   .WithName("UpdateAnPost")
		//   .Produces(204)
		//   .Produces(400)
		//   .Produces(409);

		routeGroupBuilder.MapDelete("/{id:int}", DeletePost)
		   .WithName("DeleteAnPost")
		   .Produces(204)
		   .Produces(404);

		routeGroupBuilder.MapGet("/get-posts-filter", GetFilteredPosts)
			.WithName("GetFilterPosts")
			.Produces<ApiResponse<PostDto>>();

		routeGroupBuilder.MapGet("/get-filter", GetFilter)
			.WithName("GetFilter")
			.Produces<ApiResponse<PostFilterModel>>();

		return app;
	}

	//private static async Task<IResult> GetPosts(
	//	[AsParameters] PostFilterModel model,
	//	IPostRepository postRepository,
	//	  IMapper mapper)
	//{
	//	var postQuery = mapper.Map<PostQuery>(model);
	//	var postsList = await postRepository.GetPagedPostAsync(
	//	  postQuery, model,
	//	  posts => posts.ProjectToType<PostDto>());

	//	var paginationResult = new PaginationResult<PostDto>(postsList);

	//	return Results.Ok(paginationResult);
	//}



	private static async Task<IResult> GetPostDetails(
		int id,
		IPostRepository postRepository,
		IMapper mapper)
	{
		var post = await postRepository.GetCachedPostByIdAsync(id);
		return post == null
			? Results.NotFound($"khong tim thay bai viet co ma so {id}")
			: Results.Ok(mapper.Map<PostItem>(post));
	}

	private static async Task<IResult> GetPostById(
		int id,
		[AsParameters] PagingModel pagingModel,
		IPostRepository postRepository)
	//IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery()
		{
			Id = id,
			PublishedOnly = true,
		};
		var postsList = await postRepository.GetPagedPostAsync(
			postQuery, pagingModel,
			posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postsList);
		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> GetPostsBySlug(
		[FromRoute] string slug,
		[AsParameters] PagingModel pagingModel,
		IPostRepository postRepository,
		IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery()
		{
			Slug = slug,
			PublishedOnly = true,
		};
		var postsList = await blogRepository.GetPagedPostsAsync(
			postQuery, pagingModel,
			posts => posts.ProjectToType<PostDto>());
		var paginationResult = new PaginationResult<PostDto>(postsList);
		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> AddPost(
		HttpContext context,
		IPostRepository postRepository,
		IMapper mapper,
		IMediaManager mediaManager)
	{
		//var validationResult = await validator.ValidateAsync(model);
		//if (!validationResult.IsValid)
		//{
		//	return Results.BadRequest(
		//		validationResult.Errors.ToResponse());
		//}
		//if (await postRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
		//{
		//	return Results.Conflict($"Slug'{model.UrlSlug}' da duoc su dung");
		//}
		//var post = mapper.Map<Post>(model);
		//await postRepository.AddOrUpdateAsync(post);
		//return Results.CreatedAtRoute(
		//	"GetPostById", new { post.Id },
		//	mapper.Map<PostItem>(post));
		
		var model = await PostEditModel.BindAsync(context);
		var slug = model.Title.Generate();
		if(await postRepository.IsPostSlugExistedAsync(model.Id,slug))
		{
			return Results.Ok(ApiResponse.Fail(
				HttpStatusCode.Conflict, $"Slug '{slug}' đã được sử dụng cho bài viết khác"));
		}

		var post = model.Id >0 ? await postRepository.GetPostByIdAsync(model.Id) : null;
		if(post == null)
		{
			post = new Post()
			{
				PostedDate = DateTime.Now,
			};
		}
		
		post.Title = model.Title;
		post.AuthorId = model.AuthorId;
		post.CategoryId = model.CategoryId;
		post.ShortDescription = model.ShortDescription;
		post.Description = model.Description;
		post.Meta = model.Meta;
		post.Published = model.Pulished;
		post.ModifiedDate = DateTime.Now;
		post.UrlSlug = model.Title.Generate();

		if(model.ImageFile?.Length >0)
		{
			string hostname = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}/",
				uploadPath = await mediaManager.SaveFileAsync(
					model.ImageFile.OpenReadStream(),
					model.ImageFile.FileName,
					model.ImageFile.ContentType);
			if(!string.IsNullOrWhiteSpace(uploadPath))
			{
				post.ImageUrl  = uploadPath;
			}
		}

		await postRepository.CreateOrUpdatePostAsync(post, model.GetSelectedTags());
		return Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post), HttpStatusCode.Created));

		
	}

	private static async Task<IResult> SetPostPicture(
		int id, IFormFile imageFile,
		IPostRepository postRepository,
		IMediaManager mediaManager)
	{
		var imageUrl = await mediaManager.SaveFileAsync(
			imageFile.OpenReadStream(),
			imageFile.FileName, imageFile.ContentType);
		if (string.IsNullOrWhiteSpace(imageUrl))
		{
			return Results.BadRequest("khong luu duoc tap tin");
		}
		await postRepository.SetImageUrlAsync(id, imageUrl);
		return Results.Ok(imageUrl);
	}

	//private static async Task<IResult> UpdatePost(
	//	int id, PostEditModel model,
	//	IValidator<PostEditModel> validator,
	//	IPostRepository postRepository,
	//	IMapper mapper)
	//{
	//	var validationResult = await validator.ValidateAsync(model);
	//	if (!validationResult.IsValid)
	//	{
	//		return Results.BadRequest(
	//			validationResult.Errors.ToResponse());
	//	}

	//	if (await postRepository
	//		.IsPostSlugExistedAsync(id, model.UrlSlug))
	//	{
	//		return Results.Conflict(
	//			$"Slug '{model.UrlSlug}' da duoc su dung");
	//	}
	//	var post = mapper.Map<Post>(model);
	//	post.Id = id;

	//	return await postRepository.AddOrUpdateAsync(post)
	//		? Results.NoContent()
	//		: Results.NotFound();
	//}

	private static async Task<IResult> DeletePost(
		int id, IPostRepository postRepository)
	{
		return await postRepository.DeletePostAsync(id)
			? Results.NoContent()
			: Results.NotFound($"could not find post with id={id}");
	}

	private static async Task<IResult> GetFilter(
		IAuthorRepository authorRepository,
		IBlogRepository blogRepository)
	{

		var model = new PostFilterModel()
		{
			AuthorList = (await authorRepository.GetAuthorsAsync())
			.Select(a => new SelectListItem
			{
				Text = a.FullName,
				Value = a.Id.ToString()
			}),

			CategoryList = (await blogRepository.GetCategoriesAsync())
			.Select(c =>  new SelectListItem
			{
			Text = c.Name,
				Value = c.Id.ToString()
			})
		};

		return Results.Ok(ApiResponse.Success(model));
	}


	[EnableCors("TatBlogApp")]
	private static async Task<IResult> GetFilteredPosts(
			[AsParameters] PostFilterModel model,
			[AsParameters] PagingModel pagingModel,
			IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery()
		{
			Keyword = model.Keyword,
			CategoryId = model.CategoryId,
			AuthorId = model.AuthorId,
			Year = model.Year,
			Month = model.Month,
		};

		var postList = await blogRepository.GetPagedPostsAsync(
			postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postList);
		return Results.Ok(ApiResponse.Success(paginationResult));

	}






}














