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
using BusinessLogicLayer.ViewModel;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel;

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
            testingUser = 
                new User(1, "alice123", "alice@example.com" );
            testingUser.UserType = 2; // Seller
            currentUser = new User(2, "bob321", "bob@example.com");
            currentUser.UserType = 3; //Buyer

            // Instantiate database connection
            var dataBaseConnection = new DataBaseConnection();
            
            // Instantiate repositories
            var categoryRepository = new ProductCategoryRepository(dataBaseConnection);
            var conditionRepository = new ProductConditionRepository(dataBaseConnection);
            var tagRepository = new ProductTagRepository(dataBaseConnection);
            var auctionRepository = new AuctionProductsRepository(dataBaseConnection);
            var borrowRepository = new BorrowProductsRepository(dataBaseConnection);
            var buyRepository = new BuyProductsRepository(dataBaseConnection);
            var reviewRepository = new ReviewRepository(dataBaseConnection);
            var basketRepository = new BasketRepository(dataBaseConnection);
            

            // 4. Instantiate services
            var productService = new ProductService(borrowRepository);
            var buyProductsService = new BuyProductsService(buyRepository);
            var borrowProductsService = new BorrowProductsService(borrowRepository);
            var auctionProductsService = new AuctionProductsService(auctionRepository);
            var categoryService = new ProductCategoryService(categoryRepository);
            var tagService = new ProductTagService(tagRepository);
            var conditionService = new ProductConditionService(conditionRepository);
            var reviewsService = new ReviewsService(reviewRepository);
            var basketService = new BasketService(basketRepository);

            // Instantiate view models

            buyProductsViewModel = new BuyProductsViewModel(buyProductsService);
            auctionProductsViewModel = new AuctionProductsViewModel(auctionProductsService);
            productCategoryViewModel = new ProductCategoryViewModel(categoryService);
            productTagViewModel = new ProductTagViewModel(tagService);
            productConditionViewModel = new ProductConditionViewModel(conditionService);
            borrowProductsViewModel = new BorrowProductsViewModel(borrowProductsService);
            
            auctionProductSortAndFilterViewModel = new SortAndFilterViewModel(auctionProductsService);
            borrowProductSortAndFilterViewModel = new SortAndFilterViewModel(borrowProductsService);
            buyProductSortAndFilterViewModel = new SortAndFilterViewModel(buyProductsService);
            reviewCreateViewModel = new ReviewCreateViewModel(reviewsService, currentUser, testingUser);
            seeSellerReviewsViewModel = new SeeSellerReviewsViewModel(reviewsService, testingUser, testingUser); 
            seeBuyerReviewsViewModel = new SeeBuyerReviewsViewModel(reviewsService, testingUser);
            basketViewModel = new BasketViewModel(currentUser, basketService );
        }

        private Window mainWindow;
        
        public static BuyProductsViewModel buyProductsViewModel { get; private set; }
        public static BorrowProductsViewModel borrowProductsViewModel { get; private set; }
        public static AuctionProductsViewModel auctionProductsViewModel { get; private set; }
        public static ProductCategoryViewModel productCategoryViewModel { get; private set; }
        public static ProductConditionViewModel productConditionViewModel { get; private set; }
        public static ProductTagViewModel productTagViewModel { get; private set; }
        
        public static SortAndFilterViewModel auctionProductSortAndFilterViewModel { get; private set; }
        public static SortAndFilterViewModel borrowProductSortAndFilterViewModel { get; private set; }
        public static SortAndFilterViewModel buyProductSortAndFilterViewModel { get; private set; }
        public static ReviewCreateViewModel reviewCreateViewModel { get; private set; }
        public static SeeBuyerReviewsViewModel seeBuyerReviewsViewModel { get; private set; }
        public static SeeSellerReviewsViewModel seeSellerReviewsViewModel { get; private set; }
        public static BasketViewModel basketViewModel { get; private set; }

        public static User currentUser { get; set; }
        public static User testingUser { get; set; }

    }
}
