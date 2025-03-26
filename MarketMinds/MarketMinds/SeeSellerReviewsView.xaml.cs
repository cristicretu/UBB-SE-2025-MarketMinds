using BusinessLogicLayer.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketMinds
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SeeSellerReviewsView : Window
    {
        SeeSellerReviewsViewModel ViewModel;
        public SeeSellerReviewsView(SeeSellerReviewsViewModel viewModel)
        {
            ViewModel = viewModel;
            viewModel.refreshData();
            this.InitializeComponent();
            // Show/hide elements based on review count
            ReviewsListView.Visibility = ViewModel.reviews.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            EmptyMessageTextBlock.Visibility = ViewModel.reviews.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
