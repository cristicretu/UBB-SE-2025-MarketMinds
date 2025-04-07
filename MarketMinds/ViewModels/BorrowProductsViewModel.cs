using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DomainLayer.Domain;
using MarketMinds.Services;
using MarketMinds.Services.BorrowProductsService;

namespace ViewModelLayer.ViewModel;

public class BorrowProductsViewModel
{
    private readonly BorrowProductsService borrowProductsService;
    private readonly PaginationService<BorrowProduct> paginationService;
    private readonly ProductPriceService priceService;
    private List<BorrowProduct> currentFullList;
    private int currentPage = 1;

    public ObservableCollection<BorrowProduct> BorrowProducts { get; private set; }
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

    public BorrowProductsViewModel(
        BorrowProductsService borrowProductsService,
        PaginationService<BorrowProduct> paginationService,
        ProductPriceService priceService)
    {
        this.borrowProductsService = borrowProductsService ?? throw new ArgumentNullException(nameof(borrowProductsService));
        this.paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
        this.priceService = priceService ?? throw new ArgumentNullException(nameof(priceService));
        BorrowProducts = new ObservableCollection<BorrowProduct>();
        currentFullList = new List<BorrowProduct>();
        LoadInitialProducts();
    }

    private void LoadInitialProducts()
    {
        currentFullList = GetAllProducts();
        LoadCurrentPage();
    }

    public List<BorrowProduct> GetAllProducts()
    {
        var borrowProducts = new List<BorrowProduct>();
        foreach (var product in borrowProductsService.GetProducts())
        {
            borrowProducts.Add((BorrowProduct)product);
        }
        return borrowProducts;
    }

    public void UpdateProductList(List<BorrowProduct> products)
    {
        currentFullList = products ?? throw new ArgumentNullException(nameof(products));
        currentPage = 1;
        LoadCurrentPage();
    }

    private void LoadCurrentPage()
    {
        var (pageItems, totalPages) = paginationService.GetPageItems(currentFullList, currentPage);
        TotalPages = totalPages;

        BorrowProducts.Clear();
        foreach (var item in pageItems)
        {
            BorrowProducts.Add(item);
        }
    }

    public string CalculateAndFormatPrice(BorrowProduct product, DateTime endDate)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }
        var price = priceService.CalculateBorrowPrice(product, product.StartDate, endDate);
        return priceService.FormatPrice(price);
    }

    public bool CanGoToNextPage() => paginationService.CanGoToNextPage(CurrentPage, TotalPages);
    public bool CanGoToPreviousPage() => paginationService.CanGoToPreviousPage(CurrentPage);
}