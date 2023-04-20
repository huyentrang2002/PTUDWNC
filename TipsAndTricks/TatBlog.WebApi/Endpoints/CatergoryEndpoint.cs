using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Cors;
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

public static class CategoryEndpoints
{
	//định nghĩa các API endpoint cho phần quản lý thông tin tác giả.
	public static WebApplication MapCategoryEnpoints(
		this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/categories");

		// Nested Map with defined specific route
		routeGroupBuilder.MapGet("/", GetCategories)
						 .WithName("GetCategories")
						 .Produces<ApiResponse<PaginationResult<CategoryItem>>>();

		routeGroupBuilder.MapGet("/{id:int}", GetCategoryDetails)
			.WithName("GetCategoryById")
			.Produces<ApiResponse<CategoryItem>>();
		//.Produces(404);

		routeGroupBuilder.MapGet(
			"/{slug:regex(^[a-z0-9 -]+$)}/posts",
			GetPostsByCategoriesSlug)
			.WithName("GetPostsByCategoriesSlug")
			.Produces<ApiResponse<PaginationResult<PostDto>>>();

		routeGroupBuilder.MapPost("/", AddCategory)
			.WithName("AddCategory")
			//.AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
			//.Produces(201)
			//.Produces(400)
			//.Produces(409)
			.Produces<ApiResponse<CategoryItem>>();

		routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
			.WithName("UpdateCategory")
			//.AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
			//.Produces(204)
			//.Produces(400)
			//.Produces(409);
			.Produces<ApiResponse<string>>();


		routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
			.WithName("DeleteCategory")
			//.Produces(204)
			//.Produces(404)
			.Produces<ApiResponse<string>>();

		return app;
	}

	//Xử lý yêu cầu tìm và lấy danh sách sách tác giả
	[EnableCors("TatBlogApp")]
	private static async Task<IResult> GetCategories(
		[AsParameters] CateFilterModel model,
		ICateRepository cateRepository)
	{
		var categoriesList = await
			cateRepository.GetPagedCategoriesAsync(model, model.Name);

		var paginationResult =
			new PaginationResult<CategoryItem>(categoriesList);

		return Results.Ok(ApiResponse.Success(paginationResult));
	}

	//Xem chi tiết tác giả
	private static async Task<IResult> GetCategoryDetails(
		int id,
		ICateRepository cateRepository,
		IMapper mapper)
	{
		var Category = await cateRepository.GetCachedCategoryByIdAsync(id);

		return Category == null
			? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
			$"Không tìm thấy tac giả có mã số {id}"))
			: Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(Category)));
	}

	//Lấy bài viết theo Id của Tác giả
	private static async Task<IResult> GetPostsByCategoryId(
		int id,
		[AsParameters] PagingModel pagingModel,
		IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery()
		{
			CategoryId = id,
			PublishedOnly = true,
		};

		var postList = await blogRepository.GetPagedPostsAsync(
			postQuery, pagingModel,
			posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postList);

		return Results.Ok(ApiResponse.Success(paginationResult));

	}
	//Lay bai viet theo dinh danh slug (tac gia)
	private static async Task<IResult> GetPostsByCategoriesSlug(
		[FromRoute] string slug,
		[AsParameters] PagingModel pagingModel,
		IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery()
		{
			CategorySlug = slug,
			PublishedOnly = true,
		};
		var postList = await blogRepository.GetPagedPostsAsync(
			postQuery, pagingModel,
			posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postList);

		return Results.Ok(ApiResponse.Success(paginationResult));

	}

	//Thêm tác giả
	private static async Task<IResult> AddCategory(
		CateEditModel model,
		IValidator<CateEditModel> validator,
		ICateRepository cateRepository,
		IMapper mapper)
	{
		//var validationResult = await validator.ValidateAsync(model);
		//if (validationResult.IsValid)
		//{
		//    return Results.BadRequest(
		//        validationResult.Errors.ToResponse());
		//}
		
		return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
			$"Slug '{model.UrlSlug}' đã được sử dụng"));
		

		var Category = mapper.Map<Category>(model);
		await cateRepository.AddOrUpdateAsync(Category);

		return Results.Ok(ApiResponse.Success(
			mapper.Map<CategoryItem>(Category),
			HttpStatusCode.Created));
	}

	//Cập nhật thông tin tác giả
	private static async Task<IResult> UpdateCategory(
		int id, CateEditModel model,
		IValidator<CateEditModel> validator,
		ICateRepository cateRepository,
		IMapper mapper)
	{
	
		var Category = mapper.Map<Category>(model);
		Category.Id = id;

		return await cateRepository.AddOrUpdateAsync(Category)
			? Results.Ok(ApiResponse.Success("Category is updated",
			HttpStatusCode.NoContent))
			: Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
			"Could not find Category"));
	}

	// Xóa tác giả
	private static async Task<IResult> DeleteCategory(
		int id, ICateRepository cateRepository)
	{
		return await cateRepository.DeleteCategoryAsync(id)
			? Results.Ok(ApiResponse.Success("Category is deleted",
			HttpStatusCode.NoContent))
			: Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
			"Could not find Category"));
	}
}





