using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.ViewModel;

namespace UiLayer
{
    public sealed partial class BuyProductListView : Window
    {
        private readonly BuyProductsViewModel buyProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private ObservableCollection<BuyProduct> buyProducts;

        // Pagination variables
        private int currentPage = 1;
        private int itemsPerPage = 20;
        private int totalPages = 1;
        private List<BuyProduct> currentFullList;

        public BuyProductListView()
        {
            this.InitializeComponent();

            // Assume you have a view model for buy products
            buyProductsViewModel = MarketMinds.App.buyProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.buyProductSortAndFilterViewModel;

            buyProducts = new ObservableCollection<BuyProduct>();
            currentFullList = buyProductsViewModel.GetAllProducts();
            ApplyFiltersAndPagination();
        }

        private void ApplyFiltersAndPagination()
        {
            var filteredProducts = sortAndFilterViewModel.handleSearch()
                                         .Cast<BuyProduct>().ToList();
            currentFullList = filteredProducts;
            currentPage = 1;
            totalPages = (int)Math.Ceiling(currentFullList.Count / (double)itemsPerPage);
            LoadCurrentPage();
        }

        private void LoadCurrentPage()
        {
            var pageItems = currentFullList
                                .Skip((currentPage - 1) * itemsPerPage)
                                .Take(itemsPerPage)
                                .ToList();

            buyProducts.Clear();
            foreach (var item in pageItems)
                buyProducts.Add(item);
            
            // Show the empty message if no items exist
            if (buyProducts.Count == 0)
            {
                EmptyMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyMessageTextBlock.Visibility = Visibility.Collapsed;
            }

            UpdatePaginationDisplay();
        }

        private void UpdatePaginationDisplay()
        {
            PaginationTextBlock.Text = totalPages == 0 ?
                $"Page {currentPage} of 1" :
                $"Page {currentPage} of {totalPages}";
            PreviousButton.IsEnabled = currentPage > 1;
            NextButton.IsEnabled = currentPage < totalPages;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadCurrentPage();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadCurrentPage();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            sortAndFilterViewModel.handleSearchQueryChange(SearchTextBox.Text);
            ApplyFiltersAndPagination();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterDialog filterDialog = new FilterDialog(sortAndFilterViewModel);
            filterDialog.XamlRoot = Content.XamlRoot;
            var result = await filterDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                ApplyFiltersAndPagination();
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
                    sortAndFilterViewModel.handleSortChange(sortType);
                    ApplyFiltersAndPagination();
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
                case "PriceAsc":
                    return new ProductSortType("Price", "Price", true);
                case "PriceDesc":
                    return new ProductSortType("Price", "Price", false);
                default:
                    return null;
            }
        }
    }
}
