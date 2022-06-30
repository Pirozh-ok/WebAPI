namespace Habr.Common.Parameters
{
    public abstract class QueryStringParameters
    {
        const int maxPageSize = 30;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        private int _pageSize = maxPageSize;
    }
}
