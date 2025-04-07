using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.ProductConditionService;

namespace ViewModelLayer.ViewModel
{
    public class ProductConditionViewModel : INotifyPropertyChanged
    {
        private ProductConditionService productConditionService;
        private string _conditionName;
        private string _conditionDescription;
        private string _errorMessage;
        private string _successMessage;
        private bool _isDialogOpen;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ConditionName
        {
            get => _conditionName;
            set
            {
                _conditionName = value;
                OnPropertyChanged();
            }
        }

        public string ConditionDescription
        {
            get => _conditionDescription;
            set
            {
                _conditionDescription = value;
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

        public ProductConditionViewModel(ProductConditionService productConditionService)
        {
            this.productConditionService = productConditionService;
        }
        
        public List<ProductCondition> GetAllProductConditions()
        {
            return productConditionService.GetAllProductConditions();
        }

        public ProductCondition CreateProductCondition(string displayTitle, string description)
        {
            return productConditionService.CreateProductCondition(displayTitle, description);
        }

        public void DeleteProductCondition(string displayTitle)
        {
            productConditionService.DeleteProductCondition(displayTitle);
        }

        public bool ValidateAndCreateCondition()
        {
            if (string.IsNullOrWhiteSpace(ConditionName))
            {
                ErrorMessage = "Condition name cannot be empty.";
                return false;
            }

            try
            {
                var newCondition = CreateProductCondition(ConditionName, ConditionDescription);
                SuccessMessage = $"Condition '{ConditionName}' created successfully.";
                
                // Clear input fields
                ConditionName = string.Empty;
                ConditionDescription = string.Empty;
                
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating condition: {ex.Message}";
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
