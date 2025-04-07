using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel;
using MarketMinds;
using MarketMinds.Services;
using MarketMinds.Views.Pages;

namespace UiLayer
{
    public sealed partial class BorrowProductListView : Window
    {
        private readonly BorrowProductsViewModel borrowProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private readonly CompareProductsViewModel compareProductsViewModel;

        public BorrowProductListView()
        {
            this.InitializeComponent();

            borrowProductsViewModel = MarketMinds.App.BorrowProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.BorrowProductSortAndFilterViewModel;
            compareProductsViewModel = MarketMinds.App.CompareProductsViewModel;

            BorrowListView.ItemsSource = borrowProductsViewModel.BorrowProducts;
            UpdatePaginationDisplay();
        }

        private void BorrowListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedProduct = e.ClickedItem as BorrowProduct;
            if (selectedProduct != null)
            {
                var detailView = new BorrowProductView(selectedProduct);
                detailView.Activate();
            }
        }

        private void UpdatePaginationDisplay()
        {
            PaginationTextBlock.Text = borrowProductsViewModel.TotalPages == 0 ?
                $"Page {borrowProductsViewModel.CurrentPage} of 1" :
                $"Page {borrowProductsViewModel.CurrentPage} of {borrowProductsViewModel.TotalPages}";
            
            PreviousButton.IsEnabled = borrowProductsViewModel.CanGoToPreviousPage();
            NextButton.IsEnabled = borrowProductsViewModel.CanGoToNextPage();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            borrowProductsViewModel.CurrentPage--;
            UpdatePaginationDisplay();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            borrowProductsViewModel.CurrentPage++;
            UpdatePaginationDisplay();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            sortAndFilterViewModel.HandleSearchQueryChange(SearchTextBox.Text);
            var filteredProducts = sortAndFilterViewModel.HandleSearch().Cast<BorrowProduct>().ToList();
            borrowProductsViewModel.UpdateProductList(filteredProducts);
            UpdatePaginationDisplay();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterDialog filterDialog = new FilterDialog(sortAndFilterViewModel);
            filterDialog.XamlRoot = Content.XamlRoot;
            var result = await filterDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var filteredProducts = sortAndFilterViewModel.HandleSearch().Cast<BorrowProduct>().ToList();
                borrowProductsViewModel.UpdateProductList(filteredProducts);
                UpdatePaginationDisplay();
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            SortingComboBox.Visibility = SortingComboBox.Visibility == Visibility.Visible ?
                                         Visibility.Collapsed : Visibility.Visible;
        }

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortingComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var sortTag = selectedItem.Tag.ToString();
                var sortType = ParseSortType(sortTag);
                if (sortType != null)
                {
                    sortAndFilterViewModel.HandleSortChange(sortType);
                    var filteredProducts = sortAndFilterViewModel.HandleSearch().Cast<BorrowProduct>().ToList();
                    borrowProductsViewModel.UpdateProductList(filteredProducts);
                    UpdatePaginationDisplay();
                }
            }
        }

        private ProductSortType ParseSortType(string sortTag)
        {
            switch (sortTag)
            {
                case "SellerRatingAsc":
                    return new ProductSortType("Seller Rating", "SellerRating", true);
                case "SellerRatingDesc":
                    return new ProductSortType("Seller Rating", "SellerRating", false);
                case "DailyRateAsc":
                    return new ProductSortType("Daily Rate", "DailyRate", true);
                case "DailyRateDesc":
                    return new ProductSortType("Daily Rate", "DailyRate", false);
                case "StartDateAsc":
                    return new ProductSortType("Start Date", "StartDate", true);
                case "StartDateDesc":
                    return new ProductSortType("Start Date", "StartDate", false);
                default:
                    return null;
            }
        }

        private void AddToCompare_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button.DataContext as Product;
            if (selectedProduct != null)
            {
                bool twoAdded = compareProductsViewModel.AddProductForCompare(selectedProduct);
                if (twoAdded)
                {
                    var compareProductsView = new CompareProductsView(compareProductsViewModel);
                    var compareWindow = new Window();
                    compareWindow.Content = compareProductsView;
                    compareProductsView.SetParentWindow(compareWindow);
                    compareWindow.Activate();
                }
            }
        }
    }
}
