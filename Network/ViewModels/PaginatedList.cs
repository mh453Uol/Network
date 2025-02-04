﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Network.ViewModels
{
    public class PaginatedList<T>: List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public Dictionary<string, string> AdditionalQueryStrings { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageIndex = pageIndex;
            this.AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, int? count = null)
        {
            if (!count.HasValue)
            {
                // Get the total count of records for the resource.
                count = await source.CountAsync();
            }

            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count.Value, pageIndex, pageSize);
        }

    }
}
