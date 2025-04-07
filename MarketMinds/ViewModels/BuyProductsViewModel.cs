using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DomainLayer.Domain;
using MarketMinds.Services;
using MarketMinds.Services.BuyProductsService;

namespace ViewModelLayer.ViewModel;

public class BuyProductsViewModel
{
    private readonly BuyProductsService buyProductsService;
    private readonly PaginationService<BuyProduct> paginationService;
    private readonly ProductPriceService priceService;
    private List<BuyProduct> currentFullList;
    private int currentPage = 1;

    public ObservableCollection<BuyProduct> BuyProducts { get; private set; }
    public int TotalPages { get; private set; }
    public int CurrentPage 
    { 
        get => currentPage;
        set
        {
            if (value > 0 && value <= TotalPages)
            {
                currentPage = value;
                LoadCurrentPage();
            }
        }
    }

    public BuyProductsViewModel(
        BuyProductsService buyProductsService,
        PaginationService<BuyProduct> paginationService,
        ProductPriceService priceService)
    {
        this.buyProductsService = buyProductsService ?? throw new ArgumentNullException(nameof(buyProductsService));
        this.paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
        this.priceService = priceService ?? throw new ArgumentNullException(nameof(priceService));
        BuyProducts = new ObservableCollection<BuyProduct>();
        currentFullList = new List<BuyProduct>();
        LoadInitialProducts();
    }

    private void LoadInitialProducts()
    {
        currentFullList = GetAllProducts();
        LoadCurrentPage();
    }

    public List<BuyProduct> GetAllProducts()
    {
        var buyProducts = new List<BuyProduct>();
        foreach (var product in buyProductsService.GetProducts())
        {
            buyProducts.Add((BuyProduct)product);
        }
        return buyProducts;
    }

    public void UpdateProductList(List<BuyProduct> products)
    {
        currentFullList = products ?? throw new ArgumentNullException(nameof(products));
        currentPage = 1;
        LoadCurrentPage();
    }

    private void LoadCurrentPage()
    {
        var (pageItems, totalPages) = paginationService.GetPageItems(currentFullList, currentPage);
        TotalPages = totalPages;

        BuyProducts.Clear();
        foreach (var item in pageItems)
        {
            BuyProducts.Add(item);
        }
    }

    public string CalculateAndFormatPrice(BuyProduct product, DateTime endDate)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }
        
        var price = priceService.CalculateBuyPrice(product);
        return priceService.FormatPrice(price);
    }

    public bool CanGoToNextPage() => paginationService.CanGoToNextPage(CurrentPage, TotalPages);
    public bool CanGoToPreviousPage() => paginationService.CanGoToPreviousPage(CurrentPage);
}