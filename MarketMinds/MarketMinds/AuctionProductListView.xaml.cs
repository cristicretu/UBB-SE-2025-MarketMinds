using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using DomainLayer.Domain;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ViewModelLayer.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UiLayer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuctionProductListView : Window
    {
        private readonly AuctionProductsViewModel auctionProductsViewModel;
        private ObservableCollection<AuctionProduct> auctionProducts;
        
        public AuctionProductListView()
        {
            this.InitializeComponent();
            
            auctionProductsViewModel = MarketMinds.App.auctionProductsViewModel;
            
            // Load existing data
            LoadAuctionProducts();
        }
        
        // Load existing categories from service
        private void LoadAuctionProducts()
        {
            auctionProducts.Clear();
            foreach (var product in auctionProductsViewModel.GetAllAuctionProducts())
            {
                auctionProducts.Add(product);
            }
        }
    }
}
