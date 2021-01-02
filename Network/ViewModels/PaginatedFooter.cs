using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Network.ViewModels
{
    public class PaginatedFooter
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public Dictionary<string, string> AdditionalQueryStrings { get; set; }

        public PaginatedFooter(int pageIndex, int totalPages, Dictionary<string, string> queryStrings = null)
        {
            PageIndex = pageIndex;
            TotalPages = totalPages;
            AdditionalQueryStrings = queryStrings;
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
    }
}
