using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Helper
{
    public class PaginationList<T> : List<T>
    {
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public int CurrentPage { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public int PageSize { get; set; }

        public PaginationList(int totalcount, int currentPage, int pageSize, List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalcount;
            TotalPages = (int)Math.Ceiling(totalcount / (double)pageSize);
            AddRange(items);
        }

        public static async Task<PaginationList<T>> CreateAsync(
            int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync();
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            result = result.Take(pageSize);

            var items = await result.ToListAsync();

            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }
    }
}
