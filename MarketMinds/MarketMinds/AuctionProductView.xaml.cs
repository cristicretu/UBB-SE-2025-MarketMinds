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
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketMinds
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuctionProductView : Window
    {
        private readonly AuctionProduct _product;

        public AuctionProductView(AuctionProduct product)
        {
            this.InitializeComponent();
            _product = product;
      
            LoadProductDetails();
            LoadImages();
            LoadBidHistory();
        }

        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = _product.Title;
            CategoryTextBlock.Text = _product.Category.displayTitle;
            ConditionTextBlock.Text = _product.Condition.displayTitle;
            StartingPriceTextBlock.Text = $"{_product.StartingPrice:C}";
            CurrentPriceTextBlock.Text = $"{_product.StartingPrice + 50:C}"; // Just an example
            TimeLeftTextBlock.Text = GetTimeLeft();

            // Seller Info
            SellerTextBlock.Text = _product.Seller.Username;
            DescriptionTextBox.Text = _product.Description;

            // Tags

            TagsItemsControl.ItemsSource = _product.Tags.Select(tag =>
            {
                return new TextBlock
                {
                    Text = tag.displayTitle,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4),
                    //Background = new SolidColorBrush(Windows.UI.Colors.LightGray),
                    //CornerRadius = new CornerRadius(8)
                };
            }).ToList();
        }

        private void LoadImages()
        {
            ImageCarousel.Items.Clear();

            foreach (var image in _product.Images)
            {
                Debug.WriteLine("Loading image: " + image.url);

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

        private void LoadBidHistory()
        {
            var history = new List<BidHistoryItem>
        {
            new BidHistoryItem { BidderName = "Alice", BidPrice = 150.0f, BidDate = DateTime.Now.AddMinutes(-30)},
            new BidHistoryItem { BidderName = "Bob", BidPrice = 160.0f, BidDate = DateTime.Now.AddMinutes(-10)},
        };

            BidHistoryListView.ItemsSource = history;
        }

        private string GetTimeLeft()
        {
            var timeLeft = _product.EndAuctionDate - DateTime.Now;
            return timeLeft > TimeSpan.Zero ? timeLeft.ToString(@"dd\:hh\:mm\:ss") : "Auction Ended";
        }

        private void OnPlaceBidClicked(object sender, RoutedEventArgs e)
        {
            var enteredBid = BidTextBox.Text;
            // TODO: Add bid logic
        }

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            // TODO: Show reviews
        }
    }

    public class BidHistoryItem
    {
        public string BidderName { get; set; }
        public float BidPrice { get; set; }
        public DateTime BidDate { get; set; }
    }
}
