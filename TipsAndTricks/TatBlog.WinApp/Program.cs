using TatBlog.Data.Contexts;
using TatBlog.Service.Blogs;
using TatBlog.Data.Seeders;
using TatBlog.WinApp;

//Tao doi tuong dbContext de quan ly lam viec
//voi csdl va trang thai cua cac doi tuong
var context = new BlogDbContext();
/*
//tao doi tuong khoi tao du lieu mau
var seeder = new DataSeeder(context);

//goi ham de nhap du lieu mau
seeder.Initialize();

//doc danh sach tac gia tu csdl
var authors = context.Authors.ToList();

//xuat danh sach gia ra man hinh
Console.WriteLine("{0,-4}{1,-30},{2,-30},{3,12}", "ID", "FullName", "Email", "Joined Date");

foreach (var author in authors)
{
    Console.WriteLine("{0,-4}{1,-30},{2,-30},{3,12:MM/dd/yyyy}",
        author.Id, author.FullName, author.Email, author.JoinedDate);
}

//doc danh sach bai viet tu csdl
//voi csdl va trang thai cac doi tuong
var posts = context.Posts
    .Where(p => p.Published)
    .OrderBy(p => p.Title)
    .Select(p => new
    {
        Id = p.Id,
        Title = p.Title,
        ViewCount = p.ViewCount,
        PostedDate = p.PostedDate,
        Author = p.Author,
        Category = p.Category.Name
    })
    .ToList();

//xuat danh sach va viet ra man hinh
foreach (var post in posts)
{
    Console.WriteLine("ID:      {0}", post.Id);
    Console.WriteLine("Title:   {0}", post.Title);
    Console.WriteLine("View:    {0}", post.ViewCount);
    Console.WriteLine("Date:    {0:MM/dd/yyyy}", post.PostedDate);
    Console.WriteLine("Author:  {0}", post.Author.FullName);
    Console.WriteLine("Category:{0}", post.Category);
    Console.WriteLine("".PadRight(80, '-'));
}
*/
//tao doi tuong BlogReponsitory
IBlogRepository blogRepo = new BlogRepository(context);

/*
var categories = await blogRepo.GetCategoriesAsync();
//xuat ra man hinh
Console.WriteLine("{0,-5}{1,-50}{2,10}", "ID", "Name", "Count");
foreach (var item in categories)
{
    Console.WriteLine("{0,-5}{1,-50}{2,10}",
        item.Id, item.Name, item.PostCount);
}*/

//tao doi tuong chua tham so trang
//var pagingParams = new PagingParams
//{
//    PageNumber = 1,
//    PageSize = 5,
//    SortColumn = "Name",
//    SortOrder = " DESC"
//};

////lay danh sach tu khoa
//var tagsList = await blogRepo.GetPagedTagsAsync(pagingParams);

////Xuat ra man hinh
//Console.WriteLine("{0,-5} {1,-50} {2,10}", "ID", "Name", "Count");

//foreach (var item in tagsList)
//{
//    Console.WriteLine("{0,-5} {1,-50} {2,10}",
//        item.Id,item.Name,item.PostCount);

//}



//tim 3 bai viet duoc xem nhieu nhat
var posts = await blogRepo.GetPopularArticAsync(3);

//Xuat danh sach ra man hinh

//foreach (var post in posts)
//{
//    Console.WriteLine("ID:      {0}", post.Id);
//    Console.WriteLine("Title:   {0}", post.Title);
//    Console.WriteLine("View:    {0}", post.ViewCount);
//    Console.WriteLine("Date:    {0:MM/dd/yyyy}", post.PostedDate);
//    Console.WriteLine("Author:  {0}", post.Author.FullName);
//    Console.WriteLine("Category:{0}", post.Category.Name);
//    Console.WriteLine("".PadRight(80, '-'));
//}


//a. Tìm một thẻ (Tag) theo tên định danh (slug)
var tags = await blogRepo.GetTagAsyn("google");
Console.WriteLine("{0,-5} {1,-50} {2,10}", "ID", "Name", "Description","UrlSlug");
Console.WriteLine("{0,-5} {1,-50} {2,10}",tags.Id,tags.Name,tags.Description, tags.UrlSlug);

//Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó