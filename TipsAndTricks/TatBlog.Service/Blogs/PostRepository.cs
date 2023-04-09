using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Service.Extensions;
using TatBlog.Services.Extensions;

namespace TatBlog.Service.Blogs;
public class PostRepository : IPostRepository
{
	private readonly BlogDbContext _context;
	private readonly IMemoryCache _memoryCache;

	public PostRepository(BlogDbContext context, IMemoryCache memoryCache)
	{
		_context = context;
		_memoryCache = memoryCache;
	}

	



	////Thêm và cập nhật 1 bài viết mới
	public async Task<Post> CreateOrUpdatePostAsync(
		Post post, IEnumerable<string> tags,
		CancellationToken cancellationToken = default)
	{
		if (post.Id > 0)
		{
			await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
		}
		else
		{
			post.Tags = new List<Tag>();
		}

		var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => new
			{
				Name = x,
				Slug = x.GenerateSlug()
			})
			.GroupBy(x => x.Slug)
			.ToDictionary(g => g.Key, g => g.First().Name);


		foreach (var kv in validTags)
		{
			if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

			var tag = await GetTagAsync(kv.Key, cancellationToken) ?? new Tag()
			{
				Name = kv.Value,
				Description = kv.Value,
				UrlSlug = kv.Key
			};

			post.Tags.Add(tag);
		}

		post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

		if (post.Id > 0)
			_context.Update(post);
		else
			_context.Add(post);

		await _context.SaveChangesAsync(cancellationToken);

		return post;
	}
	//Tải lên hình ảnh đại diện cho bài viết
	//public async Task<bool> SetImageUrlAsync(
	//	int postId, string imageUrl,
	//	CancellationToken cancellationToken = default)
	//{
	//	return await _context.Posts
	//		.Where(x => x.Id == postId)
	//		.ExecuteUpdateAsync(x =>
	//			x.SetProperty(a => a.ImageUrl, a => imageUrl),
	//			cancellationToken) > 0;
	//}
	

	////Xóa bài viết có mã số (id) cho trước
	//public async Task<bool> DeletePostAsync(int postId, CancellationToken cancellationToken = default)
	//{
	//	var post = await _context.Set<Post>().FindAsync(postId);

	//	if (post is null) return false;

	//	_context.Set<Post>().Remove(post);
	//	var rowsCount = await _context.SaveChangesAsync(cancellationToken);

	//	return rowsCount > 0;
	//}


	////Lấy thẻ tag (bổ trợ)
	public async Task<Tag> GetTagAsync(
		string slug, CancellationToken cancellationToken = default)
	{
		return await _context.Set<Tag>()
			.FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
	}
	public async Task<IList<TagItem>> GetTagsAsync(
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<Tag>()
			.OrderBy(x => x.Name)
			.Select(x => new TagItem()
			{
				Id = x.Id,
				Name = x.Name,
				UrlSlug = x.UrlSlug,
				Description = x.Description,
				PostCount = x.Posts.Count(p => p.Published)
			})
			.ToListAsync(cancellationToken);
	}

	//Bổ trợ
	private IQueryable<Post> FilterPosts(PostQuery condition)
	{
		IQueryable<Post> posts = _context.Set<Post>()
			.Include(x => x.Category)
			.Include(x => x.Author)
			.Include(x => x.Tags);

		if (condition.PublishedOnly)
		{
			posts = posts.Where(x => x.Published);
		}

		if (condition.NotPublished)
		{
			posts = posts.Where(x => !x.Published);
		}

		if (condition.CategoryId > 0)
		{
			posts = posts.Where(x => x.CategoryId == condition.CategoryId);
		}

		if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
		{
			posts = posts.Where(x => x.Category.UrlSlug.Contains(condition.CategorySlug));
		}

		if (condition.AuthorId > 0)
		{
			posts = posts.Where(x => x.AuthorId == condition.AuthorId);
		}

		if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
		{
			posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
		}

		if (!string.IsNullOrWhiteSpace(condition.TagSlug))
		{
			posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
		}

		if (!string.IsNullOrWhiteSpace(condition.Keyword))
		{
			posts = posts.Where(x => x.Title.Contains(condition.Keyword) ||
									 x.ShortDescription.Contains(condition.Keyword) ||
									 x.Description.Contains(condition.Keyword) ||
									 x.Category.Name.Contains(condition.Keyword) ||
									 x.Tags.Any(t => t.Name.Contains(condition.Keyword)));
		}

		if (condition.Year > 0)
		{
			posts = posts.Where(x => x.PostedDate.Year == condition.Year);
		}

		if (condition.Month > 0)
		{
			posts = posts.Where(x => x.PostedDate.Month == condition.Month);
		}

		if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
		{
			posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
		}

		return posts;
	}

	public async Task<IPagedList<T>> GetPagedPostAsync<T>(PostQuery pq, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
	{
		var posts = FilterPosts(pq);
		var mapperPosts = mapper(posts);
		return await mapperPosts
			.ToPagedListAsync(pagingParams, cancellationToken);
	}
	public async Task<Post> GetPostByIdAsync(int postId)
	{
		return await _context.Set<Post>().FindAsync(postId);
	}

	public async Task<Post> GetCachedPostByIdAsync(int postId)
	{
		return await _memoryCache.GetOrCreateAsync(
			$"category.by-id.{postId}",
			async (entry) =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
				return await GetPostByIdAsync(postId);
			});
	}
	
	public async Task<bool> AddOrUpdateAsync(
		Post post, CancellationToken cancellationToken = default)
	{
	//ost.Published = true;
		if (post.Id > 0)
		{
			_context.Posts.Update(post);
			_memoryCache.Remove($"posts.by-id.{post.Id}");
		}
		else
		{
			_context.Posts.Add(post);
		}

		return await _context.SaveChangesAsync(cancellationToken) > 0;
	}

	// giu
	public async Task<bool> DeletePostAsync(
		int postId, CancellationToken cancellationToken = default)
	{
		return await _context.Posts
			.Where(x => x.Id == postId)
			.ExecuteDeleteAsync(cancellationToken) > 0;
	}


	// giu
	public async Task<bool> IsPostSlugExistedAsync(
		int postId,
		string slug,
		CancellationToken cancellationToken = default)
	{
		return await _context.Posts
			.AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
	}


	//public async Task<IPagedList<T>> GetPagedPostAsync<T>(PostQuery pq, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
	//{
	//	var posts = FilterPost(pq);
	//	var mapperPosts = mapper(posts);
	//	return await mapperPosts
	//		.ToPagedListAsync(pagingParams, cancellationToken);
	//}
	public IQueryable<Post> FilterPost(PostQuery condition)
	{
		IQueryable<Post> posts = _context.Set<Post>()
		.Include(x => x.Category)
		.Include(x => x.Author)
		.Include(x => x.Tags);
		if (condition.PublishedOnly)
		{
			posts = posts.Where(x => x.Published);
		}

		if (condition.NotPublished)
		{
			posts = posts.Where(x => !x.Published);
		}

		if (condition.CategoryId > 0)
		{
			posts = posts.Where(x => x.CategoryId == condition.CategoryId);
		}

		if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
		{
			posts = posts.Where(x => x.Category.UrlSlug == condition.CategorySlug);
		}

		if (condition.AuthorId > 0)
		{
			posts = posts.Where(x => x.AuthorId == condition.AuthorId);
		}

		if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
		{
			posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
		}

		if (!string.IsNullOrWhiteSpace(condition.TagSlug))
		{
			posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
		}

		if (!string.IsNullOrWhiteSpace(condition.Keyword))
		{
			posts = posts.Where(x => x.Title.Contains(condition.Keyword) ||
									 x.ShortDescription.Contains(condition.Keyword) ||
									 x.Description.Contains(condition.Keyword) ||
									 x.Category.Name.Contains(condition.Keyword) ||
									 x.Tags.Any(t => t.Name.Contains(condition.Keyword)));
		}

		if (condition.Year > 0)
		{
			posts = posts.Where(x => x.PostedDate.Year == condition.Year);
		}

		if (condition.Month > 0)
		{
			posts = posts.Where(x => x.PostedDate.Month == condition.Month);
		}



		//if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
		//{
		//    posts = posts.Where(x => x.UrlSlug == condition.CategorySlug);
		//}
		if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
		{
			posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
		}

		return posts;

	}

	// giu
	public async Task<bool> SetImageUrlAsync(
		int postId, string imageUrl,
		CancellationToken cancellationToken = default)
	{
		return await _context.Posts
			.Where(x => x.Id == postId)
			.ExecuteUpdateAsync(x =>
				x.SetProperty(a => a.ImageUrl, a => imageUrl),
				cancellationToken) > 0;
	}
}





////Lấy danh sách bài viết. Hỗ trợ tìm theo từ khóa, chuyên mục, tác giả, 
//// ngày đăng, … và phân trang kết quả.

////public async Task<IList<Post>> GetPostsAsync(
////PostQuery condition,
////int pageNumber,
////int pageSize,
////CancellationToken cancellationToken = default)
////{
////	return await FilterPosts(condition)
////		.OrderByDescending(x => x.PostedDate)
////		.Skip((pageNumber - 1) * pageSize)
////		.Take(pageSize)
////		.ToListAsync(cancellationToken: cancellationToken);
////}

////Phân trang
////public async Task<IPagedList<Post>> GetPagedPostsAsync(
////	PostQuery condition,
////	int pageNumber = 1,
////	int pageSize = 10,
////	CancellationToken cancellationToken = default)
////{
////	return await FilterPosts(condition).ToPagedListAsync(
////		pageNumber, pageSize,
////		nameof(Post.PostedDate), "DESC",
////		cancellationToken);
////}

////public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
////	Func<IQueryable<Post>, IQueryable<T>> mapper,
////	IPagingParams pagingParams,
////	string name = null,
////	CancellationToken cancellationToken = default)
////{
////	var PostQuery = _context.Set<Post>().AsNoTracking();

////	if (!string.IsNullOrEmpty(name))
////	{
////		PostQuery = PostQuery.Where(x => x.Title.Contains(name));
////	}

////	return await mapper(PostQuery)
////		.ToPagedListAsync(pagingParams, cancellationToken);
////}

////public async Task<IPagedList<PostItem>> GetPagedPostsAsync(
////		IPagingParams pagingParams,
////		string name = null,
////		CancellationToken cancellationToken = default)
////{
////	IQueryable<Post> PostQuery = _context.Set<Post>().AsNoTracking();
////	if (!string.IsNullOrWhiteSpace(name))
////	{
////		PostQuery = PostQuery.Where(x => x.Title.Contains(name));
////	}
////	return await PostQuery.Select(a => new PostItem()
////	{
////		Id = a.Id,
////		Title = a.Title,
////		UrlSlug = a.UrlSlug,
////		ImageUrl = a.ImageUrl,

////	})
////		.ToPagedListAsync(pagingParams, cancellationToken);
////}

////Lay bai viet theo dinh danh slug
//public async Task<Post> GetPostBySlugAsync(
//	string slug, CancellationToken cancellationToken = default)
//{
//	return await _context.Set<Post>()
//		.FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
//}

//public async Task<Post> GetCachedPostBySlugAsync(
//	string slug, CancellationToken cancellationToken = default)
//{
//	return await _memoryCache.GetOrCreateAsync(
//		$"post.by-slug.{slug}",
//		async (entry) =>
//		{
//			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
//			return await GetPostBySlugAsync(slug, cancellationToken);
//		});
//}

//public async Task<bool> IsPostSlugExistedAsync(
//	int PostId,
//	string slug,
//	CancellationToken cancellationToken = default)
//{
//	return await _context.Posts
//		.AnyAsync(x => x.Id != PostId && x.UrlSlug == slug, cancellationToken);
//}

////Lay bai viet theo id
//public async Task<Post> GetPostByIdAsync(int PostId)
//{
//	return await _context.Set<Post>().FindAsync(PostId);
//}

//public async Task<Post> GetCachedPostByIdAsync(int PostId)
//{
//	return await _memoryCache.GetOrCreateAsync(
//		$"Post.by-id.{PostId}",
//		async (entry) =>
//		{
//			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
//			return await GetPostByIdAsync(PostId);
//		});
//}

////Đếm số lượng bài viết
//public async Task<int> CountPostsAsync(
//	PostQuery condition, CancellationToken cancellationToken = default)
//{
//	return await FilterPosts(condition).CountAsync(cancellationToken: cancellationToken);
//}

////Lấy ngẫu nhiên một danh sách N (limit) bài viết.
//public async Task<IList<Post>> GetRandomPostAsync(
//	int limit, CancellationToken cancellationToken = default)
//{
//	return await _context.Set<Post>()
//		.OrderBy(p => Guid.NewGuid())
//		.Take(limit)
//		.ToListAsync(cancellationToken);
//}

////Lấy danh sách thống kê số lượng bài viết trong N(limit) tháng gần nhất.
//public async Task<IList<PostInMonthItem>> CountMonthlyPostsAsync(
//	int numMonths, CancellationToken cancellationToken = default)
//{
//	return await _context.Set<Post>()
//		.GroupBy(x => new { x.PostedDate.Year, x.PostedDate.Month })
//		.Select(g => new PostInMonthItem()
//		{
//			Year = g.Key.Year,
//			Month = g.Key.Month,
//			Count = g.Count(x => x.Published)
//		})
//		.OrderByDescending(x => x.Year)
//		.ThenByDescending(x => x.Month)
//		.ToListAsync(cancellationToken);
//}

////Lấy thông tin chi tiết của bài viết có mã số(id) cho trước.
//public async Task<Post> GetPostByIdAsync(
//	int postId, bool includeDetails = false,
//	CancellationToken cancellationToken = default)
//{
//	if (!includeDetails)
//	{
//		return await _context.Set<Post>().FindAsync(postId);
//	}

//	return await _context.Set<Post>()
//		.Include(x => x.Category)
//		.Include(x => x.Author)
//		.Include(x => x.Tags)
//		.FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
//}


////Lấy thông tin chi tiết bài viết có tên định danh(slug) cho trước.
//public async Task<Post> GetPostAsync(
//	string slug,
//	CancellationToken cancellationToken = default)
//{
//	var postQuery = new PostQuery()
//	{
//		PublishedOnly = false,
//		TitleSlug = slug
//	};

//	return await FilterPosts(postQuery).FirstOrDefaultAsync(cancellationToken);
//}

//public async Task<bool> AddOrUpdateAsync(
//	Post post, CancellationToken cancellationToken = default)
//{
//	if (post.Id > 0)
//	{
//		_context.Posts.Update(post);
//		_memoryCache.Remove($"post.by-id.{post.Id}");
//	}
//	else
//	{
//		_context.Posts.Add(post);
//	}

//	return await _context.SaveChangesAsync(cancellationToken) > 0;
//}