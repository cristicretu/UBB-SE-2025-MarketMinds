using BusinessLogicLayer.Services;
using BusinessLogicLayer.ViewModel;
using DomainLayer.Domain;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ViewModelLayer.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketMinds
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SeeBuyerReviewsView : Window
    {
        public SeeBuyerReviewsViewModel ViewModel { get; set; }
        
        public SeeBuyerReviewsView(SeeBuyerReviewsViewModel viewModel)
        {
            ViewModel = viewModel;
            viewModel.refreshData();
            this.InitializeComponent();
            // Show/hide elements based on review count
            ReviewsListView.Visibility = ViewModel.reviews.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            EmptyMessageTextBlock.Visibility = ViewModel.reviews.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void DeleteReviewButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var review = button.DataContext as Review; // Assuming 'Review' is the model for the review

            if (review != null)
            {
                // Remove the review from the ViewModel's collection
                ViewModel.DeleteReview(review);

                // Manually refresh the visibility of the reviews
                ReviewsListView.Visibility = ViewModel.reviews.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                EmptyMessageTextBlock.Visibility = ViewModel.reviews.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void EditReviewButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var review = button.DataContext as Review; // Assuming 'Review' is the model for the review

            if (review != null)
            {
                
                var reviewCreateView = new CreateReviewView(App.ReviewCreateViewModel,review);
                reviewCreateView.Activate();
                ViewModel.refreshData();
                this.Close();
            }
        }


    }
}
