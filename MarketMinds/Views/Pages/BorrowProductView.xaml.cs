using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
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
using MarketMinds.Views.Pages;

namespace MarketMinds
{
    public sealed partial class BorrowProductView : Window
    {
        public BorrowProduct Product { get; private set; }
        private Window? seeSellerReviewsView;
        private readonly User currentUser;
        public DateTime? SelectedEndDate { get; private set; }

        public BorrowProductView(BorrowProduct product)
        {
            this.InitializeComponent();
            Product = product;
            currentUser = MarketMinds.App.CurrentUser;

            // Initialize date controls
            StartDateTextBlock.Text = Product.StartDate.ToString("d");
            TimeLimitTextBlock.Text = Product.TimeLimit.ToString("d");

            LoadProductDetails();
            LoadImages();
        }

        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = Product.Title;
            CategoryTextBlock.Text = Product.Category.DisplayTitle;
            ConditionTextBlock.Text = Product.Condition.DisplayTitle;

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
        private void OnJoinWaitListClicked(object sender, RoutedEventArgs e)
        {
        }

        private void OnLeaveWaitListClicked(object sender, RoutedEventArgs e)
        {
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

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.Seller = Product.Seller;
            // Create a window to host the SeeSellerReviewsView page
            var window = new Window();
            window.Content = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            window.Activate();
            // Store reference to window
            seeSellerReviewsView = window;
        }

        private void EndDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (EndDatePicker.Date != null)
            {
                SelectedEndDate = EndDatePicker.Date.Value.DateTime;
                CalculatePriceButton.IsEnabled = true; // Enable the Get Price button
            }
            else
            {
                SelectedEndDate = null;
                CalculatePriceButton.IsEnabled = false; // Disable if no date selected
            }
        }

        private void OnCalculatePriceClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedEndDate.HasValue)
            {
                // Calculate price based on days difference and daily rate
                TimeSpan duration = SelectedEndDate.Value - Product.StartDate;
                int days = duration.Days + 1; // Include both start and end dates
                float totalPrice = days * Product.DailyRate;

                PriceTextBlock.Text = totalPrice.ToString("C"); // Format as currency
            }
        }
    }
}