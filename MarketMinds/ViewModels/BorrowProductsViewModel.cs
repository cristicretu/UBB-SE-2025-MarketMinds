﻿using System.Collections.Generic;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel;

public class BorrowProductsViewModel
{
    private readonly BorrowProductsService borrowProductsService;

    public BorrowProductsViewModel(BorrowProductsService borrowProductsService)
    {
        this.borrowProductsService = borrowProductsService;
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
}