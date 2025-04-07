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
    public sealed partial class BorrowProductView : Window
    {
        public BorrowProduct Product { get; private set; }
        private Window? seeSellerReviewsView;
        private readonly User currentUser;
        private readonly BorrowProductsViewModel borrowProductsViewModel;
        public DateTime? SelectedEndDate { get; private set; }

        public BorrowProductView(BorrowProduct product)
        {
            this.InitializeComponent();
            Product = product;
            currentUser = MarketMinds.App.CurrentUser;
            borrowProductsViewModel = MarketMinds.App.BorrowProductsViewModel;

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

        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            MarketMinds.App.SeeSellerReviewsViewModel.Seller = Product.Seller;
            var window = new Window();
            window.Content = new SeeSellerReviewsView(MarketMinds.App.SeeSellerReviewsViewModel);
            window.Activate();
            seeSellerReviewsView = window;
        }

        private void EndDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (EndDatePicker.Date != null)
            {
                SelectedEndDate = EndDatePicker.Date.Value.DateTime;
                CalculatePriceButton.IsEnabled = true;
            }
            else
            {
                SelectedEndDate = null;
                CalculatePriceButton.IsEnabled = false;
            }
        }

        private void OnCalculatePriceClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedEndDate.HasValue)
            {
                PriceTextBlock.Text = borrowProductsViewModel.CalculateAndFormatPrice(Product, SelectedEndDate.Value);
            }
        }
    }
}