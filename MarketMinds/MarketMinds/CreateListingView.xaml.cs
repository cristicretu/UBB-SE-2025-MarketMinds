using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ViewModelLayer.ViewModel;
using System.Collections.Generic;
using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using DataAccessLayer;
using DomainLayer.Domain;
using System.Collections.ObjectModel;
using MarketMinds;
using System.Linq;
using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace UiLayer
{
    public sealed partial class CreateListingView : Page
    {
        private CreateListingViewModelBase viewModel;
        private ProductCategoryViewModel productCategoryViewModel;
        private ProductConditionViewModel productConditionViewModel;
        private ProductTagViewModel productTagViewModel;
        private TextBox titleTextBox;
        private TextBlock titleErrorTextBlock;
        private ComboBox categoryComboBox;
        private TextBlock categoryErrorTextBlock;
        private TextBox descriptionTextBox;
        private TextBlock descriptionErrorTextBlock;
        private TextBox tagsTextBox;
        private TextBlock tagsErrorTextBlock;
        private ListView tagsListView;
        private TextBox imagesTextBox;
        private ComboBox conditionComboBox;
        private TextBlock conditionErrorTextBlock;
        private ObservableCollection<string> tags;

        public CreateListingView()
        {
            this.InitializeComponent();
            titleTextBox = new TextBox { PlaceholderText = "Title" };
            titleErrorTextBlock = new TextBlock { Text = "Title cannot be empty.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            categoryComboBox = new ComboBox();
            categoryErrorTextBlock = new TextBlock { Text = "Please select a category.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            descriptionTextBox = new TextBox { PlaceholderText = "Description" };
            descriptionErrorTextBlock = new TextBlock { Text = "Description cannot be empty.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            tagsTextBox = new TextBox { PlaceholderText = "Tags" };
            tagsErrorTextBlock = new TextBlock { Text = "Please add at least one tag.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            tagsListView = new ListView();
            imagesTextBox = new TextBox { PlaceholderText = "Images" };
            conditionComboBox = new ComboBox();
            conditionErrorTextBlock = new TextBlock { Text = "Please select a condition.", Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed };
            tags = new ObservableCollection<string>();

            // Initialize ProductCategoryViewModel
            var productCategoryService = new ProductCategoryService(new ProductCategoryRepository(new DataBaseConnection()));
            productCategoryViewModel = new ProductCategoryViewModel(productCategoryService);

            // Initialize ProductConditionViewModel
            var productConditionService = new ProductConditionService(new ProductConditionRepository(new DataBaseConnection()));
            productConditionViewModel = new ProductConditionViewModel(productConditionService);

            // Initialize ProductTagViewModel
            var productTagService = new ProductTagService(new ProductTagRepository(new DataBaseConnection()));
            productTagViewModel = new ProductTagViewModel(productTagService);

            // Load categories and conditions into ComboBoxes
            LoadCategories();
            LoadConditions();

            // Set up event handler for tagsTextBox
            tagsTextBox.KeyDown += TagsTextBox_KeyDown;

            // Set up the ListView item template
            tagsListView.ItemClick += TagsListView_ItemClick;
            tagsListView.IsItemClickEnabled = true;

            tagsListView.ItemsSource = tags;
        }

        private void LoadCategories()
        {
            List<ProductCategory> categories = productCategoryViewModel.GetAllProductCategories();
            categoryComboBox.ItemsSource = categories;
            categoryComboBox.DisplayMemberPath = "displayTitle";
            categoryComboBox.SelectedValuePath = "id";
        }

        private void LoadConditions()
        {
            List<ProductCondition> conditions = productConditionViewModel.GetAllProductConditions();
            conditionComboBox.ItemsSource = conditions;
            conditionComboBox.DisplayMemberPath = "displayTitle";
            conditionComboBox.SelectedValuePath = "id";
        }

        private void ListingTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = (e.AddedItems[0] as ComboBoxItem)?.Content.ToString();
            FormContainer.Children.Clear();

            switch (selectedType)
            {
                case "Buy":
                    var BuyProductsService = new BuyProductsService(new BuyProductsRepository(new DataBaseConnection()));
                    viewModel = new CreateBuyListingViewModel { BuyProductsService = BuyProductsService };
                    AddBuyProductFields();
                    break;
                case "Borrow":
                    var BorrowProductsService = new BorrowProductsService(new BorrowProductsRepository(new DataBaseConnection()));
                    viewModel = new CreateBorrowListingViewModel { BorrowProductsService = BorrowProductsService };
                    AddBorrowProductFields();
                    break;
                case "Auction":
                    var AuctionProductsService = new AuctionProductsService(new AuctionProductsRepository(new DataBaseConnection()));
                    viewModel = new CreateAuctionListingViewModel { auctionProductsService = AuctionProductsService };
                    AddAuctionProductFields();
                    break;
            }
        }

        private void AddCommonFields()
        {
            FormContainer.Children.Add(titleTextBox);
            FormContainer.Children.Add(titleErrorTextBlock);
            FormContainer.Children.Add(categoryComboBox);
            FormContainer.Children.Add(categoryErrorTextBlock);
            FormContainer.Children.Add(descriptionTextBox);
            FormContainer.Children.Add(descriptionErrorTextBlock);
            FormContainer.Children.Add(tagsTextBox);
            FormContainer.Children.Add(tagsErrorTextBlock);
            FormContainer.Children.Add(tagsListView);
            FormContainer.Children.Add(imagesTextBox);
            FormContainer.Children.Add(conditionComboBox);
            FormContainer.Children.Add(conditionErrorTextBlock);
        }

        private void AddBuyProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Price", Name = "PriceTextBox" });
        }

        private void AddBorrowProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new CalendarDatePicker { PlaceholderText = "End Date", Name = "EndDatePicker" });
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Time Limit (Days)", Name = "TimeLimitTextBox" });
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Daily Rate", Name = "DailyRateTextBox" });
        }

        private void AddAuctionProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Starting Price", Name = "StartingPriceTextBox" });
            FormContainer.Children.Add(new CalendarDatePicker { PlaceholderText = "End Auction Date", Name = "EndAuctionDatePicker" });
        }

        private void TagsTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string tag = tagsTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(tag) && !tags.Contains(tag))
                {
                    tags.Add(tag);
                    tagsListView.ItemsSource = tags;
                    tagsTextBox.Text = string.Empty;
                }
            }
        }

        private void TagsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tag = e.ClickedItem as string;
            if (tag != null)
            {
                tags.Remove(tag);
            }
        }

        private ProductTag EnsureTagExists(string tagName)
        {
            var allTags = productTagViewModel.GetAllProductTags();
            var existingTag = allTags.FirstOrDefault(tag => tag.displayTitle.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (existingTag != null)
            {
                return existingTag;
            }
            else
            {
                return productTagViewModel.CreateProductTag(tagName);
            }
        }

        private void CreateListingButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset error messages
            titleErrorTextBlock.Visibility = Visibility.Collapsed;
            categoryErrorTextBlock.Visibility = Visibility.Collapsed;
            descriptionErrorTextBlock.Visibility = Visibility.Collapsed;
            tagsErrorTextBlock.Visibility = Visibility.Collapsed;
            conditionErrorTextBlock.Visibility = Visibility.Collapsed;

            // Collect common data
            string title = titleTextBox.Text;
            if (string.IsNullOrWhiteSpace(title))
            {
                titleErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (categoryComboBox.SelectedItem == null)
            {
                categoryErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }
            ProductCategory category = (ProductCategory)categoryComboBox.SelectedItem;

            string description = descriptionTextBox.Text;

            List<ProductTag> tags = this.tags.Select(tag => EnsureTagExists(tag)).ToList();

            if (conditionComboBox.SelectedItem == null)
            {
                conditionErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }
            ProductCondition condition = (ProductCondition)conditionComboBox.SelectedItem;

            // Collect specific data based on the selected type
            if (viewModel is CreateBuyListingViewModel)
            {
                if (!float.TryParse(((TextBox)FormContainer.FindName("PriceTextBox")).Text, out float price))
                {
                    titleErrorTextBlock.Text = "Please enter a valid price.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                var product = new BuyProduct(0, title, description, App.currentUser, condition, category, tags, new List<DomainLayer.Domain.Image>(), price);
                viewModel.CreateListing(product);
            }
            else if (viewModel is CreateBorrowListingViewModel)
            {
                if (!((CalendarDatePicker)FormContainer.FindName("EndDatePicker")).Date.HasValue)
                {
                    titleErrorTextBlock.Text = "Please select an end date.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                DateTime endDate = ((CalendarDatePicker)FormContainer.FindName("EndDatePicker")).Date.Value.DateTime;

                if (!int.TryParse(((TextBox)FormContainer.FindName("TimeLimitTextBox")).Text, out int timeLimit))
                {
                    titleErrorTextBlock.Text = "Please enter a valid time limit.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                if (!float.TryParse(((TextBox)FormContainer.FindName("DailyRateTextBox")).Text, out float dailyRate))
                {
                    titleErrorTextBlock.Text = "Please enter a valid daily rate.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                var product = new BorrowProduct(0, title, description, App.currentUser, condition, category, tags, new List<DomainLayer.Domain.Image>(), DateTime.Now, endDate, DateTime.Now.AddDays(timeLimit), dailyRate, false);
                viewModel.CreateListing(product);
            }
            else if (viewModel is CreateAuctionListingViewModel)
            {
                if (!float.TryParse(((TextBox)FormContainer.FindName("StartingPriceTextBox")).Text, out float startingPrice))
                {
                    titleErrorTextBlock.Text = "Please enter a valid starting price.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                if (!((CalendarDatePicker)FormContainer.FindName("EndAuctionDatePicker")).Date.HasValue)
                {
                    titleErrorTextBlock.Text = "Please select an end auction date.";
                    titleErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                DateTime endAuctionDate = ((CalendarDatePicker)FormContainer.FindName("EndAuctionDatePicker")).Date.Value.DateTime;

                var product = new AuctionProduct(0, title, description, App.currentUser, condition, category, tags, new List<DomainLayer.Domain.Image>(), DateTime.Now, endAuctionDate, startingPrice);
                viewModel.CreateListing(product);
            }
        }
    }
}