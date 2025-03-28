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
using MarketMinds;

namespace UiLayer
{
    public sealed partial class BorrowProductListView : Window
    {
        private readonly BorrowProductsViewModel borrowProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private ObservableCollection<BorrowProduct> borrowProducts;
        private CompareProductsViewModel compareProductsViewModel;

        // Pagination variables
        private int currentPage = 1;
        private int itemsPerPage = 20;
        private int totalPages = 1;
        private List<BorrowProduct> currentFullList;

        public BorrowProductListView()
        {
            this.InitializeComponent();

            // Assume you have similar view models for borrow products
            borrowProductsViewModel = MarketMinds.App.borrowProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.borrowProductSortAndFilterViewModel;
            compareProductsViewModel = MarketMinds.App.compareProductsViewModel;

            borrowProducts = new ObservableCollection<BorrowProduct>();
            currentFullList = borrowProductsViewModel.GetAllProducts();
            ApplyFiltersAndPagination();
        }

        private void BorrowListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedProduct = e.ClickedItem as BorrowProduct;
            if (selectedProduct != null)
            {
                // Create and show the detail view
                var detailView = new BorrowProductView(selectedProduct);
                detailView.Activate();

            }
        }

        private void ApplyFiltersAndPagination()
        {
            // Retrieve filtered and sorted products (cast as needed)
            var filteredProducts = sortAndFilterViewModel.handleSearch()
                                         .Cast<BorrowProduct>().ToList();
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

            borrowProducts.Clear();
            foreach (var item in pageItems)
                borrowProducts.Add(item);
            
            if (borrowProducts.Count == 0)
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
                bool twoAdded = compareProductsViewModel.AddProduct(selectedProduct);
                if (twoAdded == true)
                {
                    var compareView = new CompareProductsView(compareProductsViewModel);
                    compareView.Activate();
                }


            }
        }
    }
}
