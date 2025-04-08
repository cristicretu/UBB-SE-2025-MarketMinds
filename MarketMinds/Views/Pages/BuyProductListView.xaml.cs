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
using MarketMinds.Helpers;
using MarketMinds.Services.ProductPaginationService;

namespace UiLayer
{
    public sealed partial class BuyProductListView : Window
    {
        private readonly BuyProductsViewModel buyProductsViewModel;
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private readonly ProductPaginationService paginationService;
        private ObservableCollection<BuyProduct> buyProducts;
        private CompareProductsViewModel compareProductsViewModel;
        private readonly BuyProductListViewModelHelper buyProductListViewModelHelper;

        // Pagination variables
        private int currentPage = 1;
        private int totalPages = 1;
        private List<BuyProduct> currentFullList;

        public BuyProductListView()
        {
            this.InitializeComponent();

            // Initialize services and view models
            buyProductsViewModel = MarketMinds.App.BuyProductsViewModel;
            sortAndFilterViewModel = MarketMinds.App.BuyProductSortAndFilterViewModel;
            compareProductsViewModel = MarketMinds.App.CompareProductsViewModel;
            paginationService = new ProductPaginationService();
            buyProductListViewModelHelper = new BuyProductListViewModelHelper();

            buyProducts = new ObservableCollection<BuyProduct>();
            BuyListView.ItemsSource = buyProducts;
            ApplyFiltersAndPagination();
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

        private void ApplyFiltersAndPagination()
        {
            var (currentPageProducts, newTotalPages, fullList) = buyProductListViewModelHelper.GetBuyProductsPage(buyProductsViewModel, sortAndFilterViewModel, currentPage);
            currentFullList = fullList;
            totalPages = newTotalPages;
            buyProducts.Clear();
            foreach (var product in currentPageProducts)
            {
                buyProducts.Add(product);
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                ApplyFiltersAndPagination();
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                ApplyFiltersAndPagination();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                ApplyFiltersAndPagination();
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                ApplyFiltersAndPagination();
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
                var converter = new SortTypeConverterService();
                var sortType = converter.Convert(sortTag);
                if (sortType != null)
                {
                    sortAndFilterViewModel.HandleSortChange(sortType);
                    ApplyFiltersAndPagination();
                }
            }
        }

        private void AddToCompare_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product selectedProduct)
            {
                bool twoAdded = compareProductsViewModel.AddProductForCompare(selectedProduct);
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
