using Habr.Common.Parameters;
using Microsoft.EntityFrameworkCore;

namespace Habr.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source,
            int pageNumber, 
            int pageSize)
        {
            var count = source.Count();
            var items = await source
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
