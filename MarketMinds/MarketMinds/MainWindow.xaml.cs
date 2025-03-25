using MarketMinds;
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

namespace UiLayer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
            
        private void handleAuctionProductListViewButton_Click(object sender, RoutedEventArgs e)
        {
            auctionProductListViewWindow = new AuctionProductListView();
            auctionProductListViewWindow.Activate();
        }
        
        private void handleBorrowProductListViewButton_Click(object sender, RoutedEventArgs e)
        {
            borrowProductListViewWindow = new BorrowProductListView();
            borrowProductListViewWindow.Activate();
        }
        
        private void handleBuyProductListViewButton_Click(object sender, RoutedEventArgs e)
        {
            buyProductListViewWindow = new BuyProductListView();
            buyProductListViewWindow.Activate();
        }
        
        private void handleAdminViewButton_Click(object sender, RoutedEventArgs e)
        {
            adminViewWindow = new AdminView();
            adminViewWindow.Activate();
        }
        private void handleBasketViewButton_Click(object sender, RoutedEventArgs e)
        {
            basketViewWindow = new BasketView();
            basketViewWindow.Activate();
        }

        private void handleLeaveReviewButton_Click(Object sender, RoutedEventArgs e)
        {
            //leaveReviewViewWindow = new CreateReviewView();
            leaveReviewViewWindow.Activate();
        }

        private Window basketViewWindow;
        private Window auctionProductListViewWindow;
        private Window borrowProductListViewWindow;
        private Window buyProductListViewWindow;
        private Window adminViewWindow;
        private Window leaveReviewViewWindow;
    }
}
