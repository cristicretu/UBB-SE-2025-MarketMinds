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
        private readonly ProductCategoryViewModel productCategoryViewModel;
        private readonly ProductConditionViewModel productConditionViewModel;
        public ObservableCollection<ProductCategory> ProductCategories { get; private set; }
        public ObservableCollection<ProductCondition> ProductConditions { get; private set; }

        public AdminView()
        {
            this.InitializeComponent();

            productCategoryViewModel = MarketMinds.App.ProductCategoryViewModel;
            productConditionViewModel = MarketMinds.App.ProductConditionViewModel;

            ProductCategories = new ObservableCollection<ProductCategory>();
            ProductConditions = new ObservableCollection<ProductCondition>();

            // Load existing data
            LoadCategories();
            LoadConditions();
        }

        private async void HandleAddCategoryButton_Click(object sender, RoutedEventArgs e)
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
                var category = productCategoryViewModel.CreateProductCategory(name, description);
                ProductCategories.Add(category); // Update list dynamically
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

        private async void HandleAddConditionButton_Click(object sender, RoutedEventArgs e)
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
                var condition = productConditionViewModel.CreateProductCondition(name, description);
                ProductConditions.Add(condition);
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
            var categories = productCategoryViewModel.GetAllProductCategories();
            ProductCategories.Clear();
            foreach (var category in categories)
            {
                ProductCategories.Add(category);
            }
        }

        // Load existing conditions from service
        private void LoadConditions()
        {
            var conditions = productConditionViewModel.GetAllProductConditions();
            ProductConditions.Clear();
            foreach (var condition in conditions)
            {
                ProductConditions.Add(condition);
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
