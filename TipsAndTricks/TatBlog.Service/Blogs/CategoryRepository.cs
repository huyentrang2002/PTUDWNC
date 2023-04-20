using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Service.Blogs;
using TatBlog.Service.Extensions;


namespace TatBlog.Services.Blogs;

public class CategoryRepository : ICateRepository
{
	private readonly BlogDbContext _context;
	private readonly IMemoryCache _memoryCache;

	public CategoryRepository(BlogDbContext context, IMemoryCache memoryCache)
	{
		_context = context;
		_memoryCache = memoryCache;
	}

	public async Task<Category> GetCategoryAsync(
		string slug, CancellationToken cancellationToken = default)
	{
		return await _context.Set<Category>()
			.FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
	}

	public async Task<Category> GetCachedCategoryBySlugAsync(
		string slug, CancellationToken cancellationToken = default)
	{
		return await _memoryCache.GetOrCreateAsync(
			$"category.by-slug.{slug}",
			async (entry) =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
				return await GetCategoryAsync(slug, cancellationToken);
			});
	}

	public async Task<Category> GetCategoryByIdAsync(int categoryId)
	{
		return await _context.Set<Category>().FindAsync(categoryId);
	}

	public async Task<Category> GetCachedCategoryByIdAsync(int categoryId)
	{
		return await _memoryCache.GetOrCreateAsync(
			$"Category.by-id.{categoryId}",
			async (entry) =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
				return await GetCategoryByIdAsync(categoryId);
			});
	}

	public async Task<IList<CategoryItem>> GetCategorysAsync(
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<Category>()
			.OrderBy(a => a.Name)
			.Select(a => new CategoryItem()
			{
				Id = a.Id,
				Name = a.Name,
				UrlSlug = a.UrlSlug,
				PostCount = a.Posts.Count(p => p.Published)
			})
			.ToListAsync(cancellationToken);
	}

	//public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
	//	IPagingParams pagingParams,
	//	string name = null,
	//	CancellationToken cancellationToken = default)
	//{
	//	return await _context.Set<Category>()
	//			.Include(p => p.Posts)
	//			.AsNoTracking()
	//			.WhereIf(!string.IsNullOrWhiteSpace(name), x => x.Name.Contains(name))
	//			.Select(x => new CategoryItem
	//			{
	//				Id = x.Id,
	//				Name = x.Name,
	//				UrlSlug = x.UrlSlug,
	//				Description = x.Description,
	//				ShowOnMenu = x.ShowOnMenu,
	//				PostCount = x.Posts.Count(p => p.Published)
	//			})
	//			.ToPagedListAsync(pagingParams, cancellationToken);
	//}

	public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
			IPagingParams pagingParams,
			string name = null,
			CancellationToken cancellationToken = default)
	{
		IQueryable<Category> CategoryQuery = _context.Set<Category>().AsNoTracking();
		if (!string.IsNullOrWhiteSpace(name))
		{
			CategoryQuery = CategoryQuery.Where(x => x.Name.Contains(name));
		}
		return await CategoryQuery.Select(a => new CategoryItem()
		{
			Id = a.Id,
			Name = a.Name,
			Description = a.Description,
			UrlSlug = a.UrlSlug,
			PostCount = a.Posts.Count(p => p.Published)
		})
			.ToPagedListAsync(pagingParams, cancellationToken);
	}

	public async Task<IPagedList<T>> GetPagedCategorysAsync<T>(
		Func<IQueryable<Category>, IQueryable<T>> mapper,
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default)
	{
		var CategoryQuery = _context.Set<Category>().AsNoTracking();

		if (!string.IsNullOrEmpty(name))
		{
			CategoryQuery = CategoryQuery.Where(x => x.Name.Contains(name));
		}

		return await mapper(CategoryQuery)
			.ToPagedListAsync(pagingParams, cancellationToken);
	}

	public async Task<bool> AddOrUpdateAsync(
		Category Category, CancellationToken cancellationToken = default)
	{
		if (Category.Id > 0)
		{
			_context.Categories.Update(Category);
			_memoryCache.Remove($"Category.by-id.{Category.Id}");
		}
		else
		{
			_context.Categories.Add(Category);
		}

		return await _context.SaveChangesAsync(cancellationToken) > 0;
	}

	public async Task<bool> DeleteCategoryAsync(
		int CategoryId, CancellationToken cancellationToken = default)
	{
		return await _context.Categories
			.Where(x => x.Id == CategoryId)
			.ExecuteDeleteAsync(cancellationToken) > 0;
	}

	public async Task<IPagedList<T>> GetPagedCategoriesAsync<T>(Func<IQueryable<Category>, 
		IQueryable<T>> mapper, IPagingParams pagingParams, 
		string name = null, CancellationToken cancellationToken = default)
	{
		var categoryQuery = _context.Set<Category>().AsNoTracking();

		if (!string.IsNullOrEmpty(name))
		{
			categoryQuery = categoryQuery.Where(x => x.Name.Contains(name));
		}

		return await mapper(categoryQuery)
			.ToPagedListAsync(pagingParams, cancellationToken);
	}
}