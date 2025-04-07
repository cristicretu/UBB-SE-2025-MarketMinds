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
    public sealed partial class BuyProductListView : Window
    {
        private readonly BuyProductsViewModel buyProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private readonly CompareProductsViewModel compareProductsViewModel;

        public BuyProductListView()
        {
            this.InitializeComponent();

            // Initialize services and view models
            buyProductsViewModel = MarketMinds.App.BuyProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.BuyProductSortAndFilterViewModel;
            compareProductsViewModel = MarketMinds.App.CompareProductsViewModel;

            BuyListView.ItemsSource = buyProductsViewModel.BuyProducts;
            UpdatePaginationDisplay();
        }

        private void BuyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedProduct = e.ClickedItem as BuyProduct;
            if (selectedProduct != null)
            {
                var detailView = new BuyProductView(selectedProduct);
                detailView.Activate();
            }
        }

        private void UpdatePaginationDisplay()
        {
            PaginationTextBlock.Text = buyProductsViewModel.TotalPages == 0 ?
                $"Page {buyProductsViewModel.CurrentPage} of 1" :
                $"Page {buyProductsViewModel.CurrentPage} of {buyProductsViewModel.TotalPages}";
            
            PreviousButton.IsEnabled = buyProductsViewModel.CanGoToPreviousPage();
            NextButton.IsEnabled = buyProductsViewModel.CanGoToNextPage();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            buyProductsViewModel.CurrentPage--;
            UpdatePaginationDisplay();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            buyProductsViewModel.CurrentPage++;
            UpdatePaginationDisplay();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            sortAndFilterViewModel.HandleSearchQueryChange(SearchTextBox.Text);
            var filteredProducts = sortAndFilterViewModel.HandleSearch().Cast<BuyProduct>().ToList();
            buyProductsViewModel.UpdateProductList(filteredProducts);
            UpdatePaginationDisplay();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterDialog filterDialog = new FilterDialog(sortAndFilterViewModel);
            filterDialog.XamlRoot = Content.XamlRoot;
            var result = await filterDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var filteredProducts = sortAndFilterViewModel.HandleSearch().Cast<BuyProduct>().ToList();
                buyProductsViewModel.UpdateProductList(filteredProducts);
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
                    var filteredProducts = sortAndFilterViewModel.HandleSearch().Cast<BuyProduct>().ToList();
                    buyProductsViewModel.UpdateProductList(filteredProducts);
                    UpdatePaginationDisplay();
                }
            }
        }

        private ProductSortType? ParseSortType(string sortTag)
        {
            switch (sortTag)
            {
                case "SellerRatingAsc":
                    return new ProductSortType("Seller Rating", "SellerRating", true);
                case "SellerRatingDesc":
                    return new ProductSortType("Seller Rating", "SellerRating", false);
                case "PriceAsc":
                    return new ProductSortType("Price", "Price", true);
                case "PriceDesc":
                    return new ProductSortType("Price", "Price", false);
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
