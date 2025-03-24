using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using UiLayer;
using BusinessLogicLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;

namespace MarketMinds
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            mainWindow = new UiLayer.MainWindow();
            mainWindow.Activate();
            
            // Instantiate database connection
            var dataBaseConnection = new DataBaseConnection();
            
            // Instantiate repositories
            var categoryRepository = new ProductCategoryRepository(dataBaseConnection);
            var conditionRepository = new ProductConditionRepository(dataBaseConnection);
            var auctionRepository = new AuctionProductsRepository(dataBaseConnection);
            // var borrowRepository = ... de adaugat
            //var buyRepository = ... de adaugat

            // 4. Instantiate services
            auctionProductsService = new AuctionProductsService(auctionRepository);
            // buyProductsService = ... de adaugat
            // borrowProductsService = ... de adaugat

            var categoryService = new ProductCategoryService(categoryRepository);
            var conditionService = new ProductConditionService(conditionRepository);

            productCategoryViewModel = new ProductCategoryViewModel(categoryService);
            productConditionViewModel = new ProductConditionViewModel(conditionService);

            var basketRepository = new BasketRepository(dataBaseConnection);
            basketService = new BasketService(basketRepository);
        }

        private Window mainWindow;
        //public static BuyProductsService buyProductsService { get; private set; }

        //public static BorrowProductsService borrowProductsService { get; private set; }
        public static AuctionProductsService auctionProductsService { get; private set; }

        public static ProductService productService { get; private set; }
        
        public static ProductCategoryViewModel productCategoryViewModel { get; private set; }
        public static ProductConditionViewModel productConditionViewModel { get; private set; }

        public static BasketService basketService { get; private set; }
    }
}
