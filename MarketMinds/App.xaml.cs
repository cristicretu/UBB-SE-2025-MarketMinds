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
using BusinessLogicLayer.ViewModel;
using DataAccessLayer;
using DomainLayer.Domain;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.ViewModel;
using Microsoft.Extensions.Configuration;
using MarketMinds.Repositories.AuctionProductsRepository;
using MarketMinds.Repositories.BasketRepository;
using MarketMinds.Repositories.BorrowProductsRepository;
using MarketMinds.Repositories.BuyProductsRepository;
using MarketMinds.Repositories.ProductCategoryRepository;
using MarketMinds.Repositories.ProductConditionRepository;
using MarketMinds.Repositories.ProductTagRepository;
using MarketMinds.Repositories.ReviewRepository;
using MarketMinds.Services.AuctionProductsService;
using MarketMinds.Services.BorrowProductsService;
using MarketMinds.Services.BasketService;
using MarketMinds.Services.BuyProductsService;
using MarketMinds.Services.ProductCategoryService;
using MarketMinds.Services.ProductConditionService;
using MarketMinds.Services.ReviewService;
using MarketMinds.Services.ProductTagService;

namespace MarketMinds
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration configuration;
        public static DataBaseConnection dataBaseConnection;
        
        // Repository declarations
        public static ProductCategoryRepository productCategoryRepository;
        public static ProductConditionRepository productConditionRepository;
        public static ProductTagRepository productTagRepository;
        public static AuctionProductsRepository auctionProductsRepository;
        public static BorrowProductsRepository borrowProductsRepository;
        public static BuyProductsRepository buyProductsRepository;
        public static ReviewRepository reviewRepository;
        public static BasketRepository basketRepository;

        // Service declarations
        public static ProductService productService;
        public static BuyProductsService buyProductsService;
        public static BorrowProductsService borrowProductsService;
        public static AuctionProductsService auctionProductsService;
        public static ProductCategoryService categoryService;
        public static ProductTagService tagService;
        public static ProductConditionService conditionService;
        public static ReviewsService reviewsService;
        public static BasketService basketService;

        // ViewModel declarations
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
        public static CompareProductsViewModel compareProductsViewModel { get; private set; }

        public static User currentUser { get; set; }
        public static User testingUser { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeConfiguration();
            this.InitializeComponent();
        }

        private IConfiguration InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();
            return configuration;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            mainWindow = new UiLayer.MainWindow();
            mainWindow.Activate();
            
            // Create test users that match the database
            testingUser = new User(1, "alice123", "alice@example.com");
            testingUser.UserType = 1; // Matches database value
            currentUser = new User(2, "bob321", "bob@example.com");
            currentUser.UserType = 2; // Matches database value

            // Instantiate database connection with configuration
            dataBaseConnection = new DataBaseConnection(configuration);
            
            // Instantiate repositories
            productCategoryRepository = new ProductCategoryRepository(dataBaseConnection);
            productConditionRepository = new ProductConditionRepository(dataBaseConnection);
            productTagRepository = new ProductTagRepository(dataBaseConnection);
            auctionProductsRepository = new AuctionProductsRepository(dataBaseConnection);
            borrowProductsRepository = new BorrowProductsRepository(dataBaseConnection);
            buyProductsRepository = new BuyProductsRepository(dataBaseConnection);
            reviewRepository = new ReviewRepository(dataBaseConnection);
            basketRepository = new BasketRepository(dataBaseConnection);

            // Instantiate services
            productService = new ProductService(borrowProductsRepository);
            buyProductsService = new BuyProductsService(buyProductsRepository);
            borrowProductsService = new BorrowProductsService(borrowProductsRepository);
            auctionProductsService = new AuctionProductsService(auctionProductsRepository);
            categoryService = new ProductCategoryService(productCategoryRepository);
            tagService = new ProductTagService(productTagRepository);
            conditionService = new ProductConditionService(productConditionRepository);
            reviewsService = new ReviewsService(reviewRepository);
            basketService = new BasketService(basketRepository);

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
            seeBuyerReviewsViewModel = new SeeBuyerReviewsViewModel(reviewsService, currentUser);
            basketViewModel = new BasketViewModel(currentUser, basketService);
            compareProductsViewModel = new CompareProductsViewModel();
        }

        private Window mainWindow;
    }
}
