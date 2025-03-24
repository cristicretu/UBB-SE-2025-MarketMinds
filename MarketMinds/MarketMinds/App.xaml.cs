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
            var productRepository = new ProductsRepository<Product>(dataBaseConnection);

            // Instantiate services using the repositories
            categoryService = new ProductCategoryService(categoryRepository);
            conditionService = new ProductConditionService(conditionRepository);
            productService = new ProductService<Product>(productRepository);
        }

        private Window mainWindow;
        public static ProductCategoryService categoryService { get; private set; }
        public static ProductConditionService conditionService { get; private set; }
        public static ProductService<Product> productService { get; private set; }
    }
}
