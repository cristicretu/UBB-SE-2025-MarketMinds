using System.Collections.Generic;
using System.Linq;

namespace MarketMinds.Services
{
    public class PaginationService<T>
    {
        private readonly int itemsPerPage;

        public PaginationService(int itemsPerPage = 20)
        {
            this.itemsPerPage = itemsPerPage;
        }

        public (List<T> PageItems, int TotalPages) GetPageItems(List<T> items, int currentPage)
        {
            var totalPages = (int)System.Math.Ceiling(items.Count / (double)itemsPerPage);
            var pageItems = items
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            return (pageItems, totalPages);
        }

        public bool CanGoToNextPage(int currentPage, int totalPages)
        {
            return currentPage < totalPages;
        }

        public bool CanGoToPreviousPage(int currentPage)
        {
            return currentPage > 1;
        }
    }
} 