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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketMinds
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuyProductView : Window
    {
        private readonly BuyProduct _product;
        private readonly User _currentUser;
        private readonly BuyProductsViewModel _buyProductsViewModel;

        private DispatcherTimer countdownTimer;
        private Window seeSellerReviewsView;
        public BuyProductView(BuyProduct product)
        {
            this.InitializeComponent();
            _product = product;
            _currentUser = MarketMinds.App.currentUser;
            //_buyProductsViewModel = MarketMinds.App.buyProductsViewModel;
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
                //_buyProductsViewModel.AddToBasket(_product, _currentUser);
            }
            catch (Exception ex)
            {
                //ShowErrorDialog(ex.Message);
            }
        }


        private void OnSeeReviewsClicked(object sender, RoutedEventArgs e)
        {
            App.seeSellerReviewsViewModel.seller = _product.Seller;
            seeSellerReviewsView = new SeeSellerReviewsView(App.seeSellerReviewsViewModel);
            seeSellerReviewsView.Activate();
        }
    }
}