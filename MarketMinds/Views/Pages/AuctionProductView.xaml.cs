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
    public sealed partial class AuctionProductView : Window
    {
        private readonly AuctionProduct Product;
        private readonly User CurrentUser;
        private readonly AuctionProductsViewModel AuctionProductsViewModel;

        private DispatcherTimer? countdownTimer;
        private Window? seeSellerReviewsView;
        public AuctionProductView(AuctionProduct product)
        {
            this.InitializeComponent();
            Product = product;
            CurrentUser = MarketMinds.App.CurrentUser;
            AuctionProductsViewModel = MarketMinds.App.AuctionProductsViewModel;
            LoadProductDetails();
            LoadImages();
            LoadBidHistory();
            StartCountdownTimer();
        }

        private void StartCountdownTimer()
        {
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }
        private void CountdownTimer_Tick(object? sender, object e)
        {
            string timeText = GetTimeLeft();
            TimeLeftTextBlock.Text = timeText;

            if (timeText == "Auction Ended" && countdownTimer != null)
            {
                countdownTimer.Stop();
                AuctionProductsViewModel.ConcludeAuction(Product);
            }
        }
        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = Product.Title;
            CategoryTextBlock.Text = Product.Category.DisplayTitle;
            ConditionTextBlock.Text = Product.Condition.DisplayTitle;
            StartingPriceTextBlock.Text = $"{Product.StartingPrice:C}";
            CurrentPriceTextBlock.Text = $"{Product.CurrentPrice:C}"; // Just an example
            TimeLeftTextBlock.Text = GetTimeLeft();

            // Seller Info
            SellerTextBlock.Text = Product.Seller.Username;
            DescriptionTextBox.Text = Product.Description;

            TagsItemsControl.ItemsSource = Product.Tags.Select(tag =>
            {
                return new TextBlock
                {
                    Text = tag.DisplayTitle,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4),
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
            BidHistoryListView.ItemsSource = Product.BidHistory
                .OrderByDescending(b => b.Timestamp)
                .ToList();
        }
        private string GetTimeLeft()
        {
            var timeLeft = Product.EndAuctionDate - DateTime.Now;
            return timeLeft > TimeSpan.Zero ? timeLeft.ToString(@"dd\:hh\:mm\:ss") : "Auction Ended";
        }
        private void OnPlaceBidClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                AuctionProductsViewModel.PlaceBid(Product, CurrentUser, BidTextBox.Text);

                // Update UI after successful bid
                CurrentPriceTextBlock.Text = $"{Product.CurrentPrice:C}";
                LoadBidHistory(); // Refresh bid list
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private async void ShowErrorDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Bid Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.Seller = Product.Seller;
            var seeSellerReviewsView = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            seeSellerReviewsView.Activate();
        }
    }
}