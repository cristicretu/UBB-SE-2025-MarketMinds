using System.Collections.Generic;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel;

public class AuctionProductsViewModel
{
    private readonly AuctionProductsService auctionProductsService;

    public AuctionProductsViewModel(AuctionProductsService auctionProductsService)
    {
        this.auctionProductsService = auctionProductsService;
    }
    
    public List<AuctionProduct> GetAllAuctionProducts()
    {
        var auctionProducts = new List<AuctionProduct>();
        foreach (var product in auctionProductsService.GetProducts())
        {
            auctionProducts.Add((AuctionProduct)product);
        }
        return auctionProducts;
    }
}