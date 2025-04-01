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
using ViewModelLayer.ViewModel;

namespace MarketMinds
{
    public sealed partial class BorrowProductView : Window
    {
        private readonly BorrowProduct _product;
        private Window? seeSellerReviewsView;
        private readonly User _currentUser;
        public DateTime? SelectedEndDate { get; private set; }

        public BorrowProductView(BorrowProduct product)
        {
            this.InitializeComponent();
            _product = product;
            _currentUser = MarketMinds.App.CurrentUser;

            // Initialize date controls
            StartDateTextBlock.Text = _product.StartDate.ToString("d");
            TimeLimitTextBlock.Text = _product.TimeLimit.ToString("d");

            LoadProductDetails();
            LoadImages();
        }

        private void LoadProductDetails()
        {
            // Basic Info
            TitleTextBlock.Text = _product.Title;
            CategoryTextBlock.Text = _product.Category.displayTitle;
            ConditionTextBlock.Text = _product.Condition.displayTitle;

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

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            App.SeeSellerReviewsViewModel.seller = _product.Seller;
            seeSellerReviewsView = new SeeSellerReviewsView(App.SeeSellerReviewsViewModel);
            seeSellerReviewsView.Activate();
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
                TimeSpan duration = SelectedEndDate.Value - _product.StartDate;
                int days = duration.Days + 1; // Include both start and end dates
                float totalPrice = days * _product.DailyRate;

                PriceTextBlock.Text = totalPrice.ToString("C"); // Format as currency
            }
        }

    }
}