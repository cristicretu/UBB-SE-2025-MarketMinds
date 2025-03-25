using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace UiLayer
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public sealed partial class AdminView : Window
    {
        private readonly ProductCategoryViewModel _productCategoryViewModel;
        private readonly ProductConditionViewModel _productConditionViewModel;
        private ObservableCollection<ProductCategory> _productCategories;
        private ObservableCollection<ProductCondition> _productConditions;

        public AdminView()
        {
            this.InitializeComponent();
            
            _productCategoryViewModel = MarketMinds.App.productCategoryViewModel;
            _productConditionViewModel = MarketMinds.App.productConditionViewModel;

            _productCategories = new ObservableCollection<ProductCategory>();
            _productConditions = new ObservableCollection<ProductCondition>();

            // Load existing data
            LoadCategories();
            LoadConditions();
        }

        private async void handleAddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            string name = CategoryNameTextBox.Text;
            string description = CategoryDescriptionTextBox.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                await ShowContentDialog("Error", "Category name cannot be empty.");
                return;
            }

            try
            {
                var category = _productCategoryViewModel.CreateProductCategory(name, description);
                _productCategories.Add(category); // Update list dynamically
                await ShowContentDialog("Success", $"Category '{name}' created successfully.");

                // Clear input fields
                CategoryNameTextBox.Text = string.Empty;
                CategoryDescriptionTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                await ShowContentDialog("Error", $"Error creating category: {ex.Message}");
            }
        }

        private async void handleAddConditionButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ConditionNameTextBox.Text;
            string description = ConditionDescriptionTextBox.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                await ShowContentDialog("Error", "Condition name cannot be empty.");
                return;
            }

            try
            {
                var condition = _productConditionViewModel.CreateProductCondition(name, description);
                _productConditions.Add(condition);
                await ShowContentDialog("Success", $"Condition '{name}' created successfully.");

                // Clear input fields
                ConditionNameTextBox.Text = string.Empty;
                ConditionDescriptionTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                await ShowContentDialog("Error", $"Error creating condition: {ex.Message}");
            }
        }

        // Load existing categories from service
        private void LoadCategories()
        {
            var categories = _productCategoryViewModel.GetAllProductCategories();
            _productCategories.Clear();
            foreach (var category in categories)
            {
                _productCategories.Add(category);
            }
        }

        // Load existing conditions from service
        private void LoadConditions()
        {
            var conditions = _productConditionViewModel.GetAllProductConditions();
            _productConditions.Clear();
            foreach (var condition in conditions)
            {
                _productConditions.Add(condition);
            }
        }

        // Helper method to display a ContentDialog
        private async System.Threading.Tasks.Task ShowContentDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot 
            };
            await dialog.ShowAsync();
        }
    }
}
