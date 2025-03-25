using System.Collections.ObjectModel;
using BusinessLogicLayer.ViewModel;
using Microsoft.UI.Xaml.Controls;
using ViewModelLayer.ViewModel;
using DomainLayer.Domain;

namespace UiLayer
{
    public partial class FilterDialog : ContentDialog
    {
        private readonly SortAndFilterViewModel sortAndFilterViewModel;
        private readonly ProductTagViewModel productTagViewModel;
        private readonly ProductConditionViewModel productConditionViewModel;
        private readonly ProductCategoryViewModel productCategoryViewModel;

        // Optionally, use observable collections for binding if needed.
        public ObservableCollection<ProductCondition> ProductConditions { get; set; }
        public ObservableCollection<ProductCategory> ProductCategories { get; set; }
        public ObservableCollection<ProductTag> ProductTags { get; set; }

        public FilterDialog(SortAndFilterViewModel sortAndFilterViewModel)
        {
            this.InitializeComponent();
            this.sortAndFilterViewModel = sortAndFilterViewModel;

            // Initialize the view models.
            // Assuming that MarketMinds.App holds your shared view models.
            this.productConditionViewModel = MarketMinds.App.productConditionViewModel;
            this.productCategoryViewModel = MarketMinds.App.productCategoryViewModel;
            this.productTagViewModel = MarketMinds.App.productTagViewModel;

            // Initialize the observable collections from the view models.
            ProductConditions = new ObservableCollection<ProductCondition>(productConditionViewModel.GetAllProductConditions());
            ProductCategories = new ObservableCollection<ProductCategory>(productCategoryViewModel.GetAllProductCategories());
            ProductTags = new ObservableCollection<ProductTag>(productTagViewModel.GetAllProductTags());

            // Set the ItemsSource of the ListViews.
            ConditionListView.ItemsSource = ProductConditions;
            CategoryListView.ItemsSource = ProductCategories;
            TagListView.ItemsSource = ProductTags;

            // Attach the event handler for the PrimaryButtonClick event.
            this.PrimaryButtonClick += FilterDialog_PrimaryButtonClick;
        }

        private void FilterDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // When the user applies filters, update the view model with selected filters.
            foreach (ProductCondition condition in ConditionListView.SelectedItems)
            {
                sortAndFilterViewModel.handleAddProductCondition(condition);
            }
            foreach (ProductCategory category in CategoryListView.SelectedItems)
            {
                sortAndFilterViewModel.handleAddProductCategory(category);
            }
            foreach (ProductTag tag in TagListView.SelectedItems)
            {
                sortAndFilterViewModel.handleAddProductTag(tag);
            }
        }
    }
}
