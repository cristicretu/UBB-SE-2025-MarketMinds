using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace BusinessLogicLayer.ViewModel
{
    public class SortAndFilterViewModel
    {
        private ProductService productService;

        public List<ProductCondition> selectedConditions { get; set;}
        public List<ProductCategory> selectedCategories { get; set;}
        public List<ProductTag> selectedTags { get; set;}
        public ProductSortType? sortCondition { get; set; }
        public string searchQuery { get; set; }
        
        public SortAndFilterViewModel(ProductService productService)
        {
            this.productService = productService;

            this.selectedConditions = new List<ProductCondition>();
            this.selectedCategories = new List<ProductCategory>();
            this.selectedTags = new List<ProductTag>();
            this.sortCondition = null;
            this.searchQuery = "";
        }

        public List<Product> handleSearch()
        {
            return ProductService.GetSortedFilteredProducts(this.productService.GetProducts(), selectedConditions, selectedCategories, selectedTags, sortCondition, searchQuery);
        }

        public void handleClearAllFilters()
        {
            this.selectedConditions.Clear();
            this.selectedCategories.Clear();
            this.selectedTags.Clear();
            this.sortCondition = null;
            this.searchQuery = "";
        }

        public void handleSortChange(ProductSortType newSortCondition)
        {
            this.sortCondition = newSortCondition;
        }

        public void handleSearchQueryChange(string searchQuery)
        {
            this.searchQuery = searchQuery;
        }

        public void handleAddProductCondition(ProductCondition condition)
        {
            this.selectedConditions.Add(condition);
        }

        public void handleRemoveProductCondition(ProductCondition condition)
        {
            this.selectedConditions.Remove(condition);
        }

        public void handleAddProductCategory(ProductCategory category)
        {
            this.selectedCategories.Add(category);
        }

        public void handleRemoveProductCategory(ProductCategory category)
        {
            this.selectedCategories.Remove(category);
        }

        public void handleAddProductTag(ProductTag tag)
        {
            this.selectedTags.Add(tag);
        }

        public void handleRemoveProductTag(ProductTag tag)
        {
            this.selectedTags.Remove(tag);
        }
    }
}
