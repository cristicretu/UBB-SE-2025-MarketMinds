using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DomainLayer.Domain;
using Microsoft.UI.Xaml.Media.Imaging;
using ViewModelLayer.ViewModel;

namespace MarketMinds
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuyProductView : Window
    {
        private readonly BuyProduct _product;
        private readonly User _currentUser;
        private readonly BasketViewModel _basketViewModel = App.BasketViewModel;

        private Window? seeSellerReviewsView;
        public BuyProductView(BuyProduct product)
        {
            this.InitializeComponent();
            _product = product;
            _currentUser = MarketMinds.App.CurrentUser;
            LoadProductDetails();
            LoadImages();
        }



        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = _product.Title;
            CategoryTextBlock.Text = _product.Category.displayTitle;
            ConditionTextBlock.Text = _product.Condition.displayTitle;
            PriceTextBlock.Text = $"{_product.Price:C}";

            // Seller Info
            SellerTextBlock.Text = _product.Seller.Username;
            DescriptionTextBox.Text = _product.Description;

            TagsItemsControl.ItemsSource = _product.Tags.Select(tag =>
            {
                return new TextBlock
                {
                    Text = tag.displayTitle,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4),
                };
            }).ToList();
        }

        private void LoadImages()
        {
            ImageCarousel.Items.Clear();

            foreach (var image in _product.Images)
            {

                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = new BitmapImage(new Uri(image.url)),
                    Stretch = Stretch.Uniform, // ✅ shows full image without cropping
                    Height = 250,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                ImageCarousel.Items.Add(img);
            }
        }

        private void OnAddtoBascketClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                _basketViewModel.AddToBasket(_product.Id);

                // Show success notification
                BasketNotificationTip.Title = "Success";
                BasketNotificationTip.Subtitle = "Product added to basket successfully!";
                BasketNotificationTip.IconSource = new SymbolIconSource() { Symbol = Symbol.Accept };
                BasketNotificationTip.IsOpen = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add product to basket: {ex.Message}");

                // Show error notification
                BasketNotificationTip.Title = "Error";
                BasketNotificationTip.Subtitle = $"Failed to add product: {ex.Message}";
                BasketNotificationTip.IconSource = new SymbolIconSource() { Symbol = Symbol.Accept };
                BasketNotificationTip.IsOpen = true;
            }
        }

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.seller = _product.Seller;
            seeSellerReviewsView = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            seeSellerReviewsView.Activate();
        }
    }
}