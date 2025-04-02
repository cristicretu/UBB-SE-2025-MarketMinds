using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.ProductTagService;

namespace BusinessLogicLayer.ViewModel
{
    public class SortAndFilterViewModel
    {
        private ProductService productService;

        public List<ProductCondition> selectedConditions { get; set; }
        public List<ProductCategory> selectedCategories { get; set; }
        public List<ProductTag> selectedTags { get; set; }
        public ProductSortType? sortCondition { get; set; }
        public string searchQuery { get; set; }
        public SortAndFilterViewModel(ProductService productService)
        {
            this.productService = productService;

            this.selectedConditions = new List<ProductCondition>();
            this.selectedCategories = new List<ProductCategory>();
            this.selectedTags = new List<ProductTag>();
            this.sortCondition = null;
            this.searchQuery = string.Empty;
        }

        public List<Product> HandleSearch()
        {
            return ProductService.GetSortedFilteredProducts(this.productService.GetProducts(), selectedConditions, selectedCategories, selectedTags, sortCondition, searchQuery);
        }

        public void HandleClearAllFilters()
        {
            this.selectedConditions.Clear();
            this.selectedCategories.Clear();
            this.selectedTags.Clear();
            this.sortCondition = null;
            this.searchQuery = string.Empty;
        }

        public void HandleSortChange(ProductSortType newSortCondition)
        {
            this.sortCondition = newSortCondition;
        }

        public void HandleSearchQueryChange(string searchQuery)
        {
            this.searchQuery = searchQuery;
        }

        public void HandleAddProductCondition(ProductCondition condition)
        {
            this.selectedConditions.Add(condition);
        }

        public void HandleRemoveProductCondition(ProductCondition condition)
        {
            this.selectedConditions.Remove(condition);
        }

        public void HandleAddProductCategory(ProductCategory category)
        {
            this.selectedCategories.Add(category);
        }

        public void HandleRemoveProductCategory(ProductCategory category)
        {
            this.selectedCategories.Remove(category);
        }

        public void HandleAddProductTag(ProductTag tag)
        {
            this.selectedTags.Add(tag);
        }

        public void HandleRemoveProductTag(ProductTag tag)
        {
            this.selectedTags.Remove(tag);
        }
    }
}
