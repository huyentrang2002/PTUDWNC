using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Service.Blogs;

public interface IPostRepository
{

	//Task<int> CountPostsAsync(
	//	PostQuery condition, CancellationToken cancellationToken = default);

	////Task<IPagedList<Post>> GetPagedPostsAsync(
	////	PostQuery condition,
	////	int pageNumber = 1,
	////	int pageSize = 10,
	////	CancellationToken cancellationToken = default);

	////Task<IPagedList<PostItem>> GetPagedPostsAsync(
	////		IPagingParams pagingParams,
	////		string name = null,
	////		CancellationToken cancellationToken = default);

	//Task<IPagedList<T>> GetPagedPostAsync<T>(
	//		PostQuery pq,
	//		IPagingParams pagingParams,
	//		Func<IQueryable<Post>, IQueryable<T>> mapper,
	//		CancellationToken cancellationToken = default);

	//Task<IPagedList<T>> GetPagedPostsAsync<T>(
	//	Func<IQueryable<Post>, IQueryable<T>> mapper,
	//	IPagingParams pagingParams,
	//	string name = null,
	//	CancellationToken cancellationToken = default);

	//Task<Post> GetPostBySlugAsync(
	//	string slug, CancellationToken cancellationToken = default);

	//Task<Post> GetCachedPostBySlugAsync(
	//	string slug, CancellationToken cancellationToken = default);

	//Task<bool> IsPostSlugExistedAsync(
	//	int PostId,
	//	string slug,
	//	CancellationToken cancellationToken = default);

	//Task<Post> GetPostByIdAsync(int PostId);

	//Task<Post> GetCachedPostByIdAsync(int PostId);

	//Task<IList<Post>> GetRandomPostAsync(
	//	int limit, CancellationToken cancellationToken = default);

	//Task<IList<PostInMonthItem>> CountMonthlyPostsAsync(
	//	int numMonths, CancellationToken cancellationToken = default);

	//Task<bool> AddOrUpdateAsync(
	//	Post post, CancellationToken cancellationToken = default);

	//Task<bool> SetImageUrlAsync(
	//	int postId, string imageUrl,
	//	CancellationToken cancellationToken = default);
	Task<Post> CreateOrUpdatePostAsync(
		Post post, IEnumerable<string> tags,
		CancellationToken cancellationToken = default);

	//Task<bool> DeletePostAsync(int postId, CancellationToken cancellationToken = default);

	//Task<Tag> GetTagAsync(
	//	string slug, CancellationToken cancellationToken = default);

	//Task<IList<TagItem>> GetTagsAsync(
	//	CancellationToken cancellationToken = default);
	Task<Post> GetPostByIdAsync(int categoryId);

	Task<Post> GetCachedPostByIdAsync(int categoryId);

	Task<bool> AddOrUpdateAsync(
		Post post,
		CancellationToken cancellationToken = default);

	Task<bool> DeletePostAsync(
		int postId,
		CancellationToken cancellationToken = default);

	Task<bool> IsPostSlugExistedAsync(
		int postId, string slug,
		CancellationToken cancellationToken = default);

	// Task<IPagedList<Post>> GetPagedPostsAsync(PostQuery condition, IPagingParams pagingParams, CancellationToken cancellationToken = default);

	Task<IPagedList<T>> GetPagedPostAsync<T>(
		   PostQuery pq,
		   IPagingParams pagingParams,
		   Func<IQueryable<Post>, IQueryable<T>> mapper,
		   CancellationToken cancellationToken = default);

	IQueryable<Post> FilterPost(PostQuery condition);

	Task<bool> SetImageUrlAsync(
		int authorId, string imageUrl,
		CancellationToken cancellationToken = default);

	//Task<IPagedList<T>> GetPagedPostsAsync<T>(
	//    PostQuery pq,
	//    IPagingParams pagingParams, 
	//    Func<IQueryable<Post>, 
	//    IQueryable<T>> mapper,


}
