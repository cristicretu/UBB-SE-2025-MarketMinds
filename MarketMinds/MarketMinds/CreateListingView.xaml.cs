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

namespace UiLayer
{
    public sealed partial class CreateListingView : Page
    {
        private CreateListingViewModelBase viewModel;
        private ProductCategoryViewModel productCategoryViewModel;
        private ProductConditionViewModel productConditionViewModel;
        private TextBox titleTextBox;
        private ComboBox categoryComboBox;
        private TextBox descriptionTextBox;
        private TextBox tagsTextBox;
        private ListView tagsListView;
        private TextBox imagesTextBox;
        private ComboBox conditionComboBox;
        private ObservableCollection<string> tags;

        public CreateListingView()
        {
            this.InitializeComponent();
            titleTextBox = new TextBox { PlaceholderText = "Title" };
            categoryComboBox = new ComboBox();
            descriptionTextBox = new TextBox { PlaceholderText = "Description" };
            tagsTextBox = new TextBox { PlaceholderText = "Tags" };
            tagsListView = new ListView();
            imagesTextBox = new TextBox { PlaceholderText = "Images" };
            conditionComboBox = new ComboBox();
            tags = new ObservableCollection<string>();

            // Initialize ProductCategoryViewModel
            var productCategoryService = new ProductCategoryService(new ProductCategoryRepository(new DataBaseConnection()));
            productCategoryViewModel = new ProductCategoryViewModel(productCategoryService);

            // Initialize ProductConditionViewModel
            var productConditionService = new ProductConditionService(new ProductConditionRepository(new DataBaseConnection()));
            productConditionViewModel = new ProductConditionViewModel(productConditionService);

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
                    viewModel = new CreateBuyListingViewModel();
                    AddBuyProductFields();
                    break;
                case "Borrow":
                    viewModel = new CreateBorrowListingViewModel();
                    AddBorrowProductFields();
                    break;
                case "Auction":
                    viewModel = new CreateAuctionListingViewModel();
                    AddAuctionProductFields();
                    break;
            }
        }

        private void AddCommonFields()
        {
            FormContainer.Children.Add(titleTextBox);
            FormContainer.Children.Add(categoryComboBox);
            FormContainer.Children.Add(descriptionTextBox);
            FormContainer.Children.Add(tagsTextBox);
            FormContainer.Children.Add(tagsListView);
            FormContainer.Children.Add(imagesTextBox);
            FormContainer.Children.Add(conditionComboBox);
        }

        private void AddBuyProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Buy Product Name" });
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Buy Product Price" });
            // Add other BuyProduct specific fields
        }

        private void AddBorrowProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Borrow Product Name" });
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Borrow Duration" });
            // Add other BorrowProduct specific fields
        }

        private void AddAuctionProductFields()
        {
            AddCommonFields();
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Auction Product Name" });
            FormContainer.Children.Add(new TextBox { PlaceholderText = "Starting Bid" });
            // Add other AuctionProduct specific fields
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

        private void CreateListingButton_Click(object sender, RoutedEventArgs e)
        {
            // Collect data from form fields and create the listing
            // viewModel.CreateListing(product);
        }
    }
}
