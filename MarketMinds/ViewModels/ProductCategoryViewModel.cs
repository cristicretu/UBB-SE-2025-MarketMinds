using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.ProductCategoryService;

namespace ViewModelLayer.ViewModel
{
    public class ProductCategoryViewModel : INotifyPropertyChanged
    {
        private ProductCategoryService productCategoryService;
        private string _categoryName;
        private string _categoryDescription;
        private string _errorMessage;
        private string _successMessage;
        private bool _isDialogOpen;

        public event PropertyChangedEventHandler PropertyChanged;

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public string CategoryDescription
        {
            get => _categoryDescription;
            set
            {
                _categoryDescription = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                IsDialogOpen = !string.IsNullOrEmpty(value);
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged();
                IsDialogOpen = !string.IsNullOrEmpty(value);
            }
        }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                _isDialogOpen = value;
                OnPropertyChanged();
            }
        }

        public ProductCategoryViewModel(ProductCategoryService productCategoryService)
        {
            this.productCategoryService = productCategoryService;
        }

        public List<ProductCategory> GetAllProductCategories()
        {
            return productCategoryService.GetAllProductCategories();
        }

        public ProductCategory CreateProductCategory(string displayTitle, string description)
        {
            return productCategoryService.CreateProductCategory(displayTitle, description);
        }

        public void DeleteProductCategory(string displayTitle)
        {
            productCategoryService.DeleteProductCategory(displayTitle);
        }

        public bool ValidateAndCreateCategory()
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                ErrorMessage = "Category name cannot be empty.";
                return false;
            }

            try
            {
                var newCategory = CreateProductCategory(CategoryName, CategoryDescription);
                SuccessMessage = $"Category '{CategoryName}' created successfully.";
                
                // Clear input fields
                CategoryName = string.Empty;
                CategoryDescription = string.Empty;
                
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating category: {ex.Message}";
                return false;
            }
        }

        public void ClearDialogMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            IsDialogOpen = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
