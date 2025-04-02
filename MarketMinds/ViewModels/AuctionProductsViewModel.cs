using System;
using System.Collections.Generic;
using DomainLayer.Domain;
using MarketMinds.Services.AuctionProductsService;

namespace ViewModelLayer.ViewModel;

public class AuctionProductsViewModel
{
    private readonly AuctionProductsService auctionProductsService;

    public AuctionProductsViewModel(AuctionProductsService auctionProductsService)
    {
        this.auctionProductsService = auctionProductsService;
    }
    public List<AuctionProduct> GetAllProducts()
    {
        var auctionProducts = new List<AuctionProduct>();
        foreach (var product in auctionProductsService.GetProducts())
        {
            auctionProducts.Add((AuctionProduct)product);
        }
        return auctionProducts;
    }

    public void PlaceBid(AuctionProduct product, User bidder, string enteredBidText)
    {
        if (!float.TryParse(enteredBidText, out float bidAmount))
        {
            throw new Exception("Invalid bid amount");
        }

        auctionProductsService.PlaceBid(product, bidder, bidAmount);
    }

    public void ConcludeAuction(AuctionProduct product)
    {
        auctionProductsService.ConcludeAuction(product);
    }
}