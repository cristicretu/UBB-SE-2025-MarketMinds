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
using MarketMinds.Views.Pages;

namespace UiLayer
{
    public sealed partial class BuyProductListView : Window
    {
        private readonly BuyProductsViewModel buyProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private ObservableCollection<BuyProduct> buyProducts;
        private CompareProductsViewModel compareProductsViewModel;

        // Pagination variables
        private int currentPage = 1;
        private int itemsPerPage = 20;
        private int totalPages = 1;
        private List<BuyProduct> currentFullList;

        public BuyProductListView()
        {
            this.InitializeComponent();

            // Assume you have a view model for buy products
            buyProductsViewModel = MarketMinds.App.BuyProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.BuyProductSortAndFilterViewModel;
            compareProductsViewModel = MarketMinds.App.CompareProductsViewModel;

            buyProducts = new ObservableCollection<BuyProduct>();
            currentFullList = buyProductsViewModel.GetAllProducts();
            ApplyFiltersAndPagination();
        }
        private void BuyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedProduct = e.ClickedItem as BuyProduct;
            if (selectedProduct != null)
            {
                // Create and show the detail view
                var detailView = new BuyProductView(selectedProduct);
                detailView.Activate();
            }
        }
        private void ApplyFiltersAndPagination()
        {
            var filteredProducts = sortAndFilterViewModel.HandleSearch()
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
            {
                buyProducts.Add(item);
            }
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
            sortAndFilterViewModel.HandleSearchQueryChange(SearchTextBox.Text);
            ApplyFiltersAndPagination();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterDialog filterDialog = new FilterDialog(sortAndFilterViewModel);
            filterDialog.XamlRoot = Content.XamlRoot;
            var result = await filterDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ApplyFiltersAndPagination();
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
                    ApplyFiltersAndPagination();
                }
            }
        }

        private ProductSortType? ParseSortType(string sortTag)
        {
            switch (sortTag)
            {
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
            if (sender is Button button && button.DataContext is Product selectedProduct)
            {
                bool twoAdded = compareProductsViewModel.AddProduct(selectedProduct);
                if (twoAdded)
                {
                    // Create a compare view
                    var compareProductsView = new CompareProductsView(compareProductsViewModel);
                    
                    // Create a window to host the CompareProductsView page
                    var compareWindow = new Window();
                    compareWindow.Content = compareProductsView;
                    compareProductsView.SetParentWindow(compareWindow);
                    compareWindow.Activate();
                }
            }
        }
    }
}
