using System;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using MarketMinds.Views.Pages;

namespace UiLayer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuyProductView : Window
    {
        public BuyProduct Product { get; private set; }
        private readonly User currentUser;
        private readonly BasketViewModel basketViewModel;
        private readonly BuyProductsViewModel buyProductsViewModel;
        private Window? seeSellerReviewsView;

        public BuyProductView(BuyProduct product)
        {
            this.InitializeComponent();
            Product = product;
            currentUser = MarketMinds.App.CurrentUser;
            basketViewModel = MarketMinds.App.BasketViewModel;
            buyProductsViewModel = MarketMinds.App.BuyProductsViewModel;
            
            LoadProductDetails();
            LoadImages();
        }

        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = Product.Title;
            CategoryTextBlock.Text = Product.Category.DisplayTitle;
            ConditionTextBlock.Text = Product.Condition.DisplayTitle;
            PriceTextBlock.Text = $"{Product.Price:C}";

            // Seller Info
            SellerTextBlock.Text = Product.Seller.Username;
            DescriptionTextBox.Text = Product.Description;

            // Tags
            TagsItemsControl.ItemsSource = Product.Tags.Select(tag =>
            {
                return new TextBlock
                {
                    Text = tag.DisplayTitle,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4)
                };
            }).ToList();
        }

        private void LoadImages()
        {
            ImageCarousel.Items.Clear();
            foreach (var image in Product.Images)
            {
                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(image.Url)),
                    Stretch = Stretch.Uniform,
                    Height = 250,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                ImageCarousel.Items.Add(img);
            }
        }

        private void OnAddtoBascketClicked(object sender, RoutedEventArgs e)
        {
            if (basketViewModel.AddProductToBasket(Product))
            {
                // Show success message
                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Product added to basket",
                    CloseButtonText = "OK",
                    XamlRoot = Content.XamlRoot
                };
                dialog.ShowAsync();
            }
            else
            {
                // Show error message
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to add product to basket",
                    CloseButtonText = "OK",
                    XamlRoot = Content.XamlRoot
                };
                dialog.ShowAsync();
            }
        }

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            MarketMinds.App.SeeSellerReviewsViewModel.Seller = Product.Seller;
            var window = new Window();
            window.Content = new SeeSellerReviewsView(MarketMinds.App.SeeSellerReviewsViewModel);
            window.Activate();
            seeSellerReviewsView = window;
        }
    }
}