using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class PagedList<T>:List<T>
    {
        public int TotalCount { get; private set; }

        public int CurrentPage { get; private set; }

        public int PageSize { get; private set; }

        public int TotalPages { get; private set; }

        public bool HasPrevious {
            get {
                return CurrentPage > 1;
            }
        }

        public bool HasNext {
            get {
                return CurrentPage < TotalPages;
            }
        }

        public PagedList(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
        {
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> authors, int pageNumber, int pageSize)
        {
            var totalCount = authors.Count();
            var items = await authors
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize).ToListAsync();
            return new PagedList<T>(items,
                totalCount,
                pageNumber,
                pageSize);
        }

    }
}
