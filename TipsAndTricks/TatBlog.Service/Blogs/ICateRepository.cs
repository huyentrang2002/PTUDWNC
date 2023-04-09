using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Service.Blogs;

namespace TatBlog.Services.Blogs;

public interface ICateRepository
{
	Task<Category> GetCategoryAsync(
		string slug,
		CancellationToken cancellationToken = default);

	Task<Category> GetCachedCategoryBySlugAsync(
		string slug, CancellationToken cancellationToken = default);

	Task<Category> GetCategoryByIdAsync(int categoryId);

	Task<Category> GetCachedCategoryByIdAsync(int categoryId);

	Task<IList<CategoryItem>> GetCategorysAsync(
		CancellationToken cancellationToken = default);
	Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default);

	Task<IPagedList<T>> GetPagedCategoriesAsync<T>(
		Func<IQueryable<Category>, IQueryable<T>> mapper,
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default);

	Task<bool> AddOrUpdateAsync(
		Category category,
		CancellationToken cancellationToken = default);

	Task<bool> DeleteCategoryAsync(
		int categoryId,
		CancellationToken cancellationToken = default);

}