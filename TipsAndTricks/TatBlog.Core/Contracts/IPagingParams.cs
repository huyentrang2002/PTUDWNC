//Chứa các thông tin cần thiết cho việc phân trang
namespace TatBlog.Core.Contracts
{
    public interface IPagingParams
    {
        //so mau tin tren 1 trang
        int PageSize { get; set; }
        
        //so trang bat dau tu 1
        int PageNumber { get; set; }

        //ten cot muon sap xep
        string SortColumn { get; set; }

        //thu tu sap xep
        string SortOrder { get; set; }


    }
}
