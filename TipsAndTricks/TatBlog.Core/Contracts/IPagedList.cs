//Chứa kết quả phân trang
namespace TatBlog.Core.Contracts
{
    public interface IPagedList
    {

        //tong so trang(so tap con)
        int PageCount { get; }
        
        //tong so phan tu tra ve truy van
        int TotalItemCount { get; }

        //chi so trang hien tai(bat dau tu 0)
        int PageIndex { get; }

        //so luong phan tu toi da tren 1 trang
        int PageSize { get; }

        //kiem tra co trang truoc hay khong
        bool HasPreviousPage { get; }

        //kiem tra co trang tiep theo hay khong
        bool HasNextPage { get; }
        
        //kiem tra co phai trang dau tien khong
        bool IsFirstPage { get; }
        
        //kiem tra co phai trang cuoi cung khong
        bool IsLastPage { get; }

        //thu tu phan tu dau trang trong truy van(bat dau tu 1)
        int FirstItemIndex { get; }

        //thu tu phan tu cuoi trang trong truy van(bat dau tu 1)
        int LastItemIndex { get; }
    }

    public interface IPagedList<out T> : IPagedList, IEnumerable<T>
    {
        //lay phan tu tai vi tri index (bat dau tu 0)
        T this[int index] { get; }

        //dem so luong phan tu chua trong trang
        int Count { get; }

    }
}
