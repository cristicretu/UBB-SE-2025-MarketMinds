using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ViewModelLayer.ViewModel;

namespace UiLayer
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public sealed partial class AdminView : Window
    {
        private readonly ProductCategoryViewModel _categoryViewModel;
        private readonly ProductConditionViewModel _conditionViewModel;

        public AdminView()
        {
            this.InitializeComponent();

            // Use the centralized services from App
            _categoryService = MarketMinds.App.CategoryService;
            _conditionService = MarketMinds.App.ConditionService;
        }

        private void handleCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                var category = _categoryService.CreateProductCategory(name, description);
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
                var condition = _conditionService.CreateProductCondition(name, description);
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

        // Helper method to display a ContentDialog
        private async System.Threading.Tasks.Task ShowContentDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}
