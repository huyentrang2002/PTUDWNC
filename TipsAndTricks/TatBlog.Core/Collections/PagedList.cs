// thực thi giao diện IPagedList < T >
using System.Collections;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Collections
{
    public class PagedList<T> : IPagedList<T>
    {
        private readonly List<T> _subnet = new();
        public PagedList(
            IList<T> items,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItemCount = totalCount;

            _subnet.AddRange(items);
        }


        public int PageIndex { get; set; }
        public int TotalItemCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber 
        { 
            get => PageIndex + 1;
            set => PageIndex = value - 1;            
        }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 0;

                var total = TotalItemCount/ PageSize;

                if (TotalItemCount % PageSize > 0)
                    total++;
                return total;
            }
        }

        public bool HasPreviousPage => PageIndex > 0;

        public bool HasNextPage => (PageIndex < (PageCount - 1));

        public bool IsFirstPage => (PageIndex <= 0);

        public bool IsLastPage => (PageIndex >= (PageCount - 1));

        public int FirstItemIndex => (PageIndex * PageSize) + 1;

        public int LastItemIndex => 
            Math.Min(TotalItemCount,(PageIndex *PageSize) + PageSize);

        #region
        public IEnumerator<T> GetEnumerator()
        {
            return _subnet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public T this[int index] => _subnet[index];
        public virtual int Count => _subnet.Count;
      
        #endregion
    }
}
